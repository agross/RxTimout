using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;

using Minimod.RxMessageBroker;

namespace RxTimeout
{

  public partial class Form1 : Form
  {
      int _offset;
      private Label _label;

    public Form1()
    {
        InitializeComponent();

        RxMessageBrokerMinimod.Default.Register<Start>(Start, TaskPoolScheduler.Default);

        var displayMessageStream = Observable.Empty<Message>();
        displayMessageStream = RxMessageBrokerMinimod.Default.Stream
                                  .OfType<Message>()
                                  .ObserveOn(new ControlScheduler(this))
                                  .Do(_ =>
                                      {
                                          if (_label == null) return;
                                          Controls.Remove(_label);
                                          _label.Dispose();
                                      })
                                  .Do(CreateAndDisplayLabel)
                                  .Timeout(TimeSpan.FromSeconds(5))
                                  .ObserveOn(new ControlScheduler(this))
                                  .Catch((Exception error) =>
                                      {
                                          Controls.Remove(_label);
                                          _label.Dispose();
                                          return displayMessageStream;
                                      });
        displayMessageStream.Subscribe();



    }

    void Start(Start obj)
    {
      RxMessageBrokerMinimod.Default.Send(new Message("1"));
      RxMessageBrokerMinimod.Default.Send(new Message("2"));
      RxMessageBrokerMinimod.Default.Send(new Message("3"));
    }

    void CreateAndDisplayLabel(Message message)
    {
      Debug.WriteLine(DateTimeOffset.Now.Ticks + " > " + message.Value);

      _label = CreateLabel();

      _label.Text = message.Value;

      _label.Size = _label.GetPreferredSize(Size);
      if (_label.Width < ClientRectangle.Width)
      {
        _label.Width += 15;
      }
      _label.Location = new Point(1, ClientRectangle.Height - _label.Height - 1 - _offset);
      _offset += 15;
      _label.Visible = true;
      Controls.Add(_label);
      Controls.SetChildIndex(_label, 0);

      //RemoveOnTimeoutOrNextMessage(label);

      Debug.WriteLine(DateTimeOffset.Now.Ticks + " < " + message.Value);
    }

    void button1_Click(object sender, EventArgs e)
    {
      RxMessageBrokerMinimod.Default.Send(new Start());
    }

    static Label CreateLabel()
    {
      return new Label
             {
               Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
               Name = "message" + Guid.NewGuid(),
               Padding = new Padding(2),
               TextAlign = ContentAlignment.BottomLeft,
               UseMnemonic = false,
               Visible = false
             };
    }
  }

  class Start
  {
  }

  class Message
  {
    public Message(string value)
    {
      Value = value;
    }

    public string Value { get; set; }
    public Control Label { get; set; }
  }
}
