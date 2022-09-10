using System;
using System.Threading.Tasks;

namespace Casbin.Persist
{
    public interface IWatcher
    {
        void SetUpdateCallback(Action callback);

        void SetUpdateCallback(Func<Task> callback);

        void Update(IWatcherMessage watcherMessage);

        Task UpdateAsync(IWatcherMessage watcherMessage);
    }
}
