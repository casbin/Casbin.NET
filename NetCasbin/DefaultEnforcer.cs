using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class DefaultEnforcer
    {
        public static IEnforcer Create(IAdapter adapter = null)
        {
            return new Enforcer(DefaultModel.Create(), adapter);
        }

        public static IEnforcer Create(string modelPath, string policyPath)
        {
            return new Enforcer(modelPath, policyPath);
        }

        public static IEnforcer Create(string modelPath, IAdapter adapter = null)
        {
            return new Enforcer(modelPath, adapter);
        }

        public static IEnforcer Create(IModel model, IAdapter adapter = null)
        {
            return new Enforcer(model, adapter);
        }
    }
}
