using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class DefaultEnforcer
    {
        public static IEnforcer Create(IReadOnlyAdapter adapter = null, bool lazyLoadPolicy = false)
        {
            return new Enforcer(DefaultModel.Create(), adapter, lazyLoadPolicy);
        }

        public static IEnforcer Create(string modelPath, string policyPath, bool lazyLoadPolicy = false)
        {
            return new Enforcer(modelPath, policyPath, lazyLoadPolicy);
        }

        public static IEnforcer Create(string modelPath, IReadOnlyAdapter adapter = null, bool lazyLoadPolicy = false)
        {
            return new Enforcer(modelPath, adapter, lazyLoadPolicy);
        }

        public static IEnforcer Create(IModel model, IReadOnlyAdapter adapter = null, bool lazyLoadPolicy = false)
        {
            return new Enforcer(model, adapter, lazyLoadPolicy);
        }
    }
}
