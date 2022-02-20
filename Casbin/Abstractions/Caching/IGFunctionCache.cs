namespace Casbin.Caching;

public interface IGFunctionCache
{
    public void Set(string name1, string name2, bool result, string domain = null);

    public bool TryGet(string name1, string name2, out bool result, string domain = null);

    public void Clear();
}
