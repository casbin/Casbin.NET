using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist;

public static class AdapterExtension
{
    public static void LoadPolicy(this IEpochAdapter adapter, IModel model) =>
        adapter.LoadPolicy(model.PolicyStoreHolder.PolicyStore);

    public static Task LoadPolicyAsync(this IEpochAdapter adapter, IModel model) =>
        adapter.LoadPolicyAsync(model.PolicyStoreHolder.PolicyStore);

    public static void SavePolicy(this IEpochAdapter adapter, IModel model) =>
        adapter.SavePolicy(model.PolicyStoreHolder.PolicyStore);

    public static Task SavePolicyAsync(this IEpochAdapter adapter, IModel model) =>
        adapter.SavePolicyAsync(model.PolicyStoreHolder.PolicyStore);
}
