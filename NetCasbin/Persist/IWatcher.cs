using System;
using System.Threading.Tasks;

namespace NetCasbin.Persist
{
    public interface IWatcher
    {
        void SetUpdateCallback(Action callback);

        void SetUpdateCallback(Func<Task> callback);

        void Update();

        Task UpdateAsync();
    }
}
