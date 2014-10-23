using System;

namespace Utils
{
  public sealed class ConsoleLogger : ILogger
  {
    private readonly object locker;

    public ConsoleLogger()
    {
      locker = new object();
    }

    public void LogMessage(string message)
    {
      if (message == null)
      {
        throw new ArgumentNullException();
      }

      lock (locker)
      {
        Console.WriteLine(message);
      }
    }
  }
}
