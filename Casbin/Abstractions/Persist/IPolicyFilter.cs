using System.Linq;

namespace Casbin.Persist;

public interface IPolicyFilter
{
    public IQueryable<T> Apply<T>(IQueryable<T> policies) where T : IPersistPolicy;
}




