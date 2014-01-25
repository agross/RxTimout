namespace RxTimeout.Messages
{
  class Message
  {
    public Message(string text)
    {
      Text = text;
    }

    public string Text { get; set; }
  }
}
