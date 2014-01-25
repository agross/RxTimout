using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Forms;

using Minimod.RxMessageBroker;

using Message = RxTimeout.Messages.Message;

namespace RxTimeout.Forms
{
  public partial class AlexForm : Form
  {
    readonly Offset _offset;

    public AlexForm()
    {
      InitializeComponent();
      _offset = new Offset(this);

      RxMessageBrokerMinimod.Default.Register<Message>(CreateAndDisplayLabel, new ControlScheduler(this));
    }

    void CreateAndDisplayLabel(Message message)
    {
      Debug.WriteLine(DateTimeOffset.Now.Ticks + " Alex> " + message.Text);

      var label = Label.Create(message.Text, this);
      _offset.Apply(label);
      
      RemoveOnTimeoutOrNextMessage(label);

      Debug.WriteLine(DateTimeOffset.Now.Ticks + " Alex< " + message.Text);
    }

    void RemoveOnTimeoutOrNextMessage(Control control)
    {
      const long ItDoesNotMatter = 42L;

      var timeout = Observable.Timer(TimeSpan.FromSeconds(5));
      var nextMessage = RxMessageBrokerMinimod.Default.Stream
                                              .OfType<Message>()
                                              .Select(_ => ItDoesNotMatter)
                                              .FirstAsync();

      nextMessage
        .Amb(timeout)
        .ObserveOn(this)
        .Do(_ =>
        {
          Debug.WriteLine("Alex> remove " + control.Text);
          Controls.Remove(control);
          control.Dispose();
        })
        .Subscribe();
    }
  }
}
