using System.Collections.Generic;

namespace Casbin.Caching;

public class GFunctionCache : IGFunctionCache
{
    private readonly Dictionary<string, bool> _cache = new();

    public void Set(string name1, string name2, bool result, string domain = null)
    {
        _cache[Key(name1, name2, domain)] = result;
    }

    public bool TryGet(string name1, string name2, out bool result, string domain = null)
    {
        return _cache.TryGetValue(Key(name1, name2, domain), out result);
    }

    private static string Key(string name1, string name2, string domain = null)
    {
        bool hasDomain = domain is not null;
        return hasDomain
            ? string.Join(":", name1, name2, domain)
            : string.Join(":", name1, name2);
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
