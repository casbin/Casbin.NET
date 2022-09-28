using Casbin.Persist;

namespace Casbin.Model.Holder;

public class WatcherHolder
{
    private IReadOnlyWatcher _watcher;

    public IReadOnlyWatcher Watcher
    {
        get => _watcher;
        set
        {
            _watcher = value;
            DetermineWater(value);
        }
    }

    public IFullWatcher FullWatcher { get; private set; }
    public IIncrementalWatcher IncrementalWatcher { get; private set; }
    public IWatcherEx WatcherEx { get; private set; }

    private void DetermineWater(IReadOnlyWatcher watcher)
    {
        FullWatcher = watcher as IFullWatcher;
        IncrementalWatcher = watcher as IIncrementalWatcher;
        WatcherEx = watcher as IWatcherEx;
    }
}
