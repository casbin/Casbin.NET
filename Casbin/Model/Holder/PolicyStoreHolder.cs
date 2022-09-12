using System.Threading;

namespace Casbin.Model.Holder;

public class PolicyStoreHolder
{
    private IPolicyStore _policyStore;

    public IPolicyStore PolicyStore
    {
        get => _policyStore;
        set => Interlocked.Exchange(ref _policyStore, value);
    }
}
