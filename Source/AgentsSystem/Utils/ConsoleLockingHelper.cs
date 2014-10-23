using System;

namespace Utils
{
  public static class ConsoleLockingHelper
  {
    private static readonly object locker;

    static ConsoleLockingHelper()
    {
      locker = new object();
    }

    public static void WriteLine(string msg)
    {
      if (msg == null)
      {
        throw new ArgumentNullException();
      }

      lock (locker)
      {
        Console.WriteLine(msg);
      }
    }

    public static void Write(string msg)
    {
      if (msg == null)
      {
        throw new ArgumentNullException();
      }

      lock (locker)
      {
        Console.Write(msg);
      }
    }
  }
}
