using System.Collections.Generic;

namespace Casbin
{
    public struct EnforceContext
    {
        internal EnforceContext(EnforceView view, bool explain = false)
        {
            View = view;
            HandleCached = false;

            Explain = explain;
            Explanations = explain ? new List<IEnumerable<string>>() : null;
        }

        public EnforceView View { get; }
        public bool HandleCached { get; internal set; }
        public bool Explain { get; }
        public List<IEnumerable<string>> Explanations { get; }

        public static EnforceContext Create(IEnforcer enforcer, bool explain)
        {
            return Create(
                enforcer,
                PermConstants.DefaultRequestType,
                PermConstants.DefaultPolicyType,
                PermConstants.DefaultPolicyEffectType,
                PermConstants.DefaultMatcherType,
                explain
            );
        }

        public static EnforceContext Create(
            IEnforcer enforcer,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            string matcherType = PermConstants.DefaultMatcherType,
            bool explain = false)
        {
            return new EnforceContext(EnforceView.Create(enforcer.Model,
                requestType, policyType, effectType, matcherType), explain);
        }

        public static EnforceContext CreateWithMatcher(IEnforcer enforcer, string matcher, bool explain)
        {
            return CreateWithMatcher(
                enforcer,
                matcher,
                PermConstants.DefaultRequestType,
                PermConstants.DefaultPolicyType,
                PermConstants.DefaultPolicyEffectType,
                explain);
        }

        public static EnforceContext CreateWithMatcher(
            IEnforcer enforcer,
            string matcher,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            bool explain = false)
        {
            return new EnforceContext(EnforceView.CreateWithMatcher(enforcer.Model,
                matcher, requestType, policyType, effectType), explain);
        }
    }
}
