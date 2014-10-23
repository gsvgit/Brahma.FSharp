using System;
using System.IO;

namespace Utils
{
  public sealed class FileLogger : ILogger
  {
    private readonly StreamWriter fileStream;
    private readonly object locker;

    public FileLogger(string fileName)
    {
      fileStream = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate));
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
        fileStream.WriteLine(message);
      }
    }
  }
}
