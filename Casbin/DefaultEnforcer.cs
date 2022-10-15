using System;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class DefaultEnforcer
    {
        public static IEnforcer Create(IReadOnlyAdapter adapter = null, Action<IEnforcer.EnforcerOptions> optionSettings = null)
        {
            return new Enforcer(DefaultModel.Create(), adapter, optionSettings);
        }

        public static IEnforcer Create(string modelPath, string policyPath, Action<IEnforcer.EnforcerOptions> optionSettings = null)
        {
            return new Enforcer(modelPath, policyPath, optionSettings);
        }

        public static IEnforcer Create(string modelPath, IReadOnlyAdapter adapter = null, Action<IEnforcer.EnforcerOptions> optionSettings = null)
        {
            return new Enforcer(modelPath, adapter, optionSettings);
        }

        public static IEnforcer Create(IModel model, IReadOnlyAdapter adapter = null, Action<IEnforcer.EnforcerOptions> optionSettings = null)
        {
            return new Enforcer(model, adapter, optionSettings);
        }
    }
}
