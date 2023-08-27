using System.Threading.Tasks;

namespace Casbin.Persist;

public interface IReadOnlyWatcher
{
    void Close();

    Task CloseAsync();
}
