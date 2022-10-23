using System.Linq;

namespace Casbin.Persist;

public interface IPolicyFilter : IPolicyFilter<IPersistantPolicy>
{
}

public interface IPolicyFilter<T> where T : IPersistantPolicy
{
    public IQueryable<T> ApplyFilter(IQueryable<T> policies);
}

