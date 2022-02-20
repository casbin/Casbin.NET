using System;
using System.Collections.Generic;
using System.Threading;

namespace Casbin.Caching;

public class EnforceViewCache : IEnforceViewCache
{
    public TimeSpan WaitTime { get; set; } = TimeSpan.FromMilliseconds(50);
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<string, EnforceView> _views = new();

    public bool TryAdd(string name, EnforceView view)
    {
        if (view is null)
        {
            return true;
        }

        if (_lock.TryEnterWriteLock(WaitTime) is false)
        {
            return false;
        }

        try
        {
            _views[name] = view;
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool TryGet(string name, out EnforceView view)
    {
        if (_lock.TryEnterReadLock(WaitTime) is false)
        {
            view = null;
            return false;
        }

        try
        {
            return _views.TryGetValue(name, out view);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}
