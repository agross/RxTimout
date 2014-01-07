using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Forms;

using Minimod.RxMessageBroker;

namespace RxTimeout
{
    public partial class Form1 : Form
    {
        int _offset;

        public Form1()
        {
            InitializeComponent();

            //RxMessageBrokerMinimod.Default.Register<CreateDisplayMessage>(CreateDisplayMessage, new ControlScheduler(this));
            RxMessageBrokerMinimod.Default.Register<Start>(Start, TaskPoolScheduler.Default);

            RxMessageBrokerMinimod.Default.Stream
                .OfType<Message>()
                .ObserveOn(new ControlScheduler(this))
                
                .Do(message =>
                    {
                        //Alternative könnte hier auch ein Select auf CreateDisplayMessage(...) gemacht werden
                        message.DisplayMessage = CreateDisplayMessage(message);
                    })
                .Window(1)
                .Do(x => x
                    .Delay(TimeSpan.FromSeconds(5))
                    .ObserveOn(new ControlScheduler(this))
                    .Do(p =>
                    {
                        Controls.Remove(p.DisplayMessage);
                        p.DisplayMessage.Dispose();
                    })
                    .Subscribe())
                .Subscribe();
        }

        void Start(Start obj)
        {
            RxMessageBrokerMinimod.Default.Send(new Message("1"));
            RxMessageBrokerMinimod.Default.Send(new Message("2"));
            RxMessageBrokerMinimod.Default.Send(new Message("3"));
        }

        Control CreateDisplayMessage(Message message)
        {
            Debug.WriteLine(DateTimeOffset.Now.Ticks + " > " + message.Value);

            var label = CreateLabel();

            label.Text = message.Value;

            label.Size = label.GetPreferredSize(Size);
            if (label.Width < ClientRectangle.Width)
            {
                label.Width += 15;
            }
            label.Location = new Point(1, ClientRectangle.Height - label.Height - 1 - _offset);
            _offset += 15;
            label.Visible = true;
            Controls.Add(label);
            Controls.SetChildIndex(label, 0);

            Debug.WriteLine(DateTimeOffset.Now.Ticks + " < " + message.Value);
            return label;
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
        public Control DisplayMessage { get; set; }
    }
}