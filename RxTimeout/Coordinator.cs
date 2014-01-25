using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows.Forms;

using Minimod.RxMessageBroker;

using RxTimeout.Messages;

using Message = RxTimeout.Messages.Message;

namespace RxTimeout
{
  class Coordinator : IDisposable
  {
    readonly IDisposable[] _subscription;

    public Coordinator()
    {
      var mainThread = new SynchronizationContextScheduler(new WindowsFormsSynchronizationContext());

      _subscription = new[]
                      {
                        RxMessageBrokerMinimod.Default.Register<Start>(Start, TaskPoolScheduler.Default),
                        RxMessageBrokerMinimod.Default.Register<SwitchToMainThread>(SwitchToMainThread, mainThread)
                      };
    }

    public void Dispose()
    {
      _subscription.ToList().ForEach(x => x.Dispose());
    }

    void Start(Start message)
    {
      RxMessageBrokerMinimod.Default.Send(new Message("Switching to long-running action that" +
                                                      " needs to run on the main thread"));
      RxMessageBrokerMinimod.Default.Send(new SwitchToMainThread());
    }

    void SwitchToMainThread(SwitchToMainThread message)
    {
      RxMessageBrokerMinimod.Default.Send(new Message("Hello from main thread"));
      Thread.Sleep(10000);
      RxMessageBrokerMinimod.Default.Send(new Message("Goodbye from main thread"));
    }
  }
}
