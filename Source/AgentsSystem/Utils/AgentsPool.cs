using System.Collections.Generic;
using System.Threading;

namespace Utils
{
  public sealed class AgentsPool<TAgent>
  {
    private readonly List<TAgent> pool;
    private readonly object locker;

    public AgentsPool()
    {
      pool = new List<TAgent>();
      locker = new object();
    }

    public int AgentsCount
    {
      get
      {
        lock (locker)
        {
          return pool.Count;
        }
      }
    }

    public bool HasAgents
    {
      get
      {
        lock (locker)
        {
          return pool.Count > 0;
        }
      }
    }

    public void AddAgent(TAgent agent)
    {
      lock (locker)
      {
        pool.Add(agent);
        Monitor.PulseAll(locker);
      }
    }

    public TAgent GetAgent()
    {
      lock (locker)
      {
        while (pool.Count <= 0)
        {
          Monitor.Wait(locker);
        }

        var data = pool[pool.Count - 1];
        pool.RemoveAt(pool.Count - 1);

        return data;
      }
    }
  }
}
