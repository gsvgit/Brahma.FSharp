namespace Utils
{
  public interface IProgressPresenter
  {
    float GetProgress { get; }
    void FireProgress(float progress);
  }
}
