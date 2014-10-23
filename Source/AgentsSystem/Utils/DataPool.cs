using System.Collections.Generic;
using System.Threading;

namespace Utils
{
  public sealed class DataPool<TData>
  {
    private readonly List<TData> pool;

    private readonly object locker;

    public DataPool()
    {
      pool = new List<TData>();
      locker = new object();
    }

    public int DataCount
    {
      get
      {
        lock (locker)
        {
          return pool.Count;
        }
      }
    }

    public bool HasData
    {
      get
      {
        lock (locker)
        {
          return pool.Count > 0;
        }
      }
    }

    public void AddData(TData data)
    {
      lock (locker)
      {
        pool.Add(data);
        Monitor.PulseAll(locker);
      }
    }

    public TData GetData()
    {
      lock (locker)
      {
        while (!HasData)
        {
          Monitor.Wait(locker);
        }
//        Console.WriteLine("Pool: {0}", pool.Count);
        var data = pool[pool.Count - 1];
        pool.RemoveAt(pool.Count - 1);

        return data;
      }
    }
  }
}
