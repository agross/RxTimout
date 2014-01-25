using Minimod.RxMessageBroker;

using RxTimeout.Messages;

namespace RxTimeout
{
  class Coordinator
  {
    public void Start()
    {
      RxMessageBrokerMinimod.Default.Send(new Message("1"));
      RxMessageBrokerMinimod.Default.Send(new Message("2"));
      RxMessageBrokerMinimod.Default.Send(new Message("3"));
    }
  }
}