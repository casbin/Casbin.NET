namespace Casbin.Caching;

public interface IGFunctionCachePool
{
    public IGFunctionCache GetCache(string roleType);

    public void Clear(string roleType);
}
