using System;

namespace Utils
{
  public class ConsoleProgressPresenter : IProgressPresenter
  {
    public float GetProgress { get; private set; }

    public ConsoleProgressPresenter()
    {
      GetProgress = 0;
    }

    public void FireProgress(float progress)
    {
      GetProgress += progress;
      Console.WriteLine("Progress = {0}", GetProgress);
    }
  }
}
