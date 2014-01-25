using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Forms;

using Minimod.RxMessageBroker;

using Message = RxTimeout.Messages.Message;

namespace RxTimeout.Forms
{
  public partial class MikesForm : Form
  {
    readonly Offset _offset;
    readonly IDisposable _subscription;
    System.Windows.Forms.Label _label;

    public MikesForm()
    {
      InitializeComponent();
      _offset = new Offset(this);

      var displayMessageStream = Observable.Empty<Message>();
      displayMessageStream = RxMessageBrokerMinimod.Default.Stream
                                                   .OfType<Message>()
                                                   .ObserveOn(new ControlScheduler(this))
                                                   .Do(_ =>
                                                   {
                                                     if (_label == null)
                                                     {
                                                       return;
                                                     }
                                                     Controls.Remove(_label);
                                                     _label.Dispose();
                                                   })
                                                   .Do(CreateAndDisplayLabel)
                                                   .Timeout(TimeSpan.FromSeconds(5))
                                                   .ObserveOn(new ControlScheduler(this))
                                                   .Catch((Exception error) =>
                                                   {
                                                     Debug.WriteLine("Mike> remove " + _label.Text);
                                                     if (_label != null)
                                                     {
                                                       Controls.Remove(_label);
                                                       _label.Dispose();
                                                     }
                                                     return displayMessageStream;
                                                   });
      _subscription = displayMessageStream.Subscribe();
    }

    void CreateAndDisplayLabel(Message message)
    {
      Debug.WriteLine("Mike> " + message.Text);

      _label = Label.Create(message.Text, this);
      _offset.Apply(_label);

      Debug.WriteLine("Mike< " + message.Text);
    }

    void OnFormClosed(object sender, EventArgs e)
    {
      _subscription.Dispose();
    }
  }
}
