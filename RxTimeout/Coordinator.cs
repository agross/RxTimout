using System;
using System.Reactive.Concurrency;
using System.Threading;

using Minimod.RxMessageBroker;

using RxTimeout.Messages;

namespace RxTimeout
{
  class Coordinator : IDisposable
  {
    readonly IDisposable _subscription;

    public Coordinator()
    {
      _subscription = RxMessageBrokerMinimod.Default.Register<Start>(Start, TaskPoolScheduler.Default);
    }

    public void Dispose()
    {
      _subscription.Dispose();
    }

    void Start(Start message)
    {
      RxMessageBrokerMinimod.Default.Send(new Message("1"));
      Wait();
      RxMessageBrokerMinimod.Default.Send(new Message("2"));
      Wait();
      RxMessageBrokerMinimod.Default.Send(new Message("3"));
    }

    static void Wait()
    {
      //Thread.Sleep(500);
    }
  }
}
