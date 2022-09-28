using System;
using System.Threading.Tasks;

namespace Casbin.Persist;

public interface IIncrementalWatcher
{
    void SetUpdateCallback(Action<PolicyChangedMessage> callback);

    void SetUpdateCallback(Func<PolicyChangedMessage, Task> callback);

    void Update(PolicyChangedMessage message);

    Task UpdateAsync(PolicyChangedMessage message);
}
