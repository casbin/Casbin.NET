namespace Casbin.Persist
{
    public interface IWatcher : IReadOnlyWatcher, IFullWatcher, IIncrementalWatcher
    {
    }
}
