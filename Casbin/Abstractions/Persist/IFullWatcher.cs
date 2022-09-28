using System;
using System.Threading.Tasks;

namespace Casbin.Persist;

public interface IFullWatcher
{
    void SetUpdateCallback(Action callback);

    void SetUpdateCallback(Func<Task> callback);

    void Update();

    Task UpdateAsync();
}
