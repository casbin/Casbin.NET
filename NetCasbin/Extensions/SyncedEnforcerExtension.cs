using Casbin.Model;

namespace Casbin.Extensions
{
    public static class SyncedEnforcerExtension
    {
        public static IEnforcer ToSyncedEnforcer(this IEnforcer enforcer)
        {
            enforcer.Model ??= DefaultModel.Create();
            enforcer.Model.ReplacePolicyManager(ReaderWriterPolicyManager.Create());
            return enforcer;
        }
    }
}
