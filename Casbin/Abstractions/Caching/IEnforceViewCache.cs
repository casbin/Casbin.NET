namespace Casbin.Caching;

public interface IEnforceViewCache
{
    public bool TryAdd(string name, EnforceView view);

    public bool TryGet(string name, out EnforceView view);
}
