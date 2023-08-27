using System;
using System.Threading.Tasks;

namespace Casbin.Persist;

public interface IIncrementalWatcher
{
    void SetUpdateCallback(Action<IPolicyChangeMessage> callback);

    void SetUpdateCallback(Func<IPolicyChangeMessage, Task> callback);

    void Update(IPolicyChangeMessage message);

    Task UpdateAsync(IPolicyChangeMessage message);
}
