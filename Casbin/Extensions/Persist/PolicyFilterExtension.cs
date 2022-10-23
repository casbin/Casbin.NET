using System.Linq;

namespace Casbin.Persist;

public static class PolicyFilterExtension
{
    public static IPolicyFilter<T> And<T>(this IPolicyFilter<T> filter1, IPolicyFilter<T> filter2)
        where T : IPersistantPolicy =>
        new PolicyFilter<T>(p => filter1.ApplyFilter(filter1.ApplyFilter(p)));

    public static IPolicyFilter<T> Or<T>(this IPolicyFilter<T> filter1, IPolicyFilter<T> filter2)
        where T : IPersistantPolicy =>
        new PolicyFilter<T>(p => filter1.ApplyFilter(p).Union(filter2.ApplyFilter(p)));
}

