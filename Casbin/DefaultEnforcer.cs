using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class DefaultEnforcer
    {
        public static IEnforcer Create(IReadOnlyAdapter adapter = null, bool autoLoadPolicy = true, Filter filter = null)
        {
            return new Enforcer(DefaultModel.Create(), adapter, autoLoadPolicy, filter);
        }

        public static IEnforcer Create(string modelPath, string policyPath, bool autoLoadPolicy = true, Filter filter = null)
        {
            return new Enforcer(modelPath, policyPath, autoLoadPolicy, filter);
        }

        public static IEnforcer Create(string modelPath, IReadOnlyAdapter adapter = null, bool autoLoadPolicy = true, Filter filter = null)
        {
            return new Enforcer(modelPath, adapter, autoLoadPolicy, filter);
        }

        public static IEnforcer Create(IModel model, IReadOnlyAdapter adapter = null, bool autoLoadPolicy = true, Filter filter = null)
        {
            return new Enforcer(model, adapter, autoLoadPolicy, filter);
        }
    }
}
