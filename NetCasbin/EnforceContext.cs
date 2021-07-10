using System.Collections.Generic;
using Casbin.Model;

namespace Casbin
{
    public readonly struct EnforceContext
    {
        public EnforceContext(IReadOnlyDictionary<string, int> requestTokens,
            IReadOnlyDictionary<string, int> policyTokens,
            IReadOnlyList<IReadOnlyList<string>> policies,
            string effect, string matcher, bool explain)
        {
            RequestTokens = requestTokens;
            PolicyTokens = policyTokens;
            Policies = policies;
            Effect = effect;
            Matcher = matcher;
            Explain = explain;
            Explanations = explain ? new List<IEnumerable<string>>() : null;
        }

        public IReadOnlyDictionary<string, int> RequestTokens { get; }
        public IReadOnlyDictionary<string, int> PolicyTokens { get; }
        public IReadOnlyList<IReadOnlyList<string>> Policies { get; }
        public string Effect { get; }
        public string Matcher { get; }
        public bool Explain { get; }
        public List<IEnumerable<string>> Explanations { get; }

        public static EnforceContext Create(IEnforcer enforcer, bool explain)
        {
            return Create(
                enforcer,
                requestType: PermConstants.DefaultRequestType,
                policyType: PermConstants.DefaultPolicyType,
                effectType: PermConstants.DefaultPolicyEffectType,
                matcherType: PermConstants.DefaultMatcherType,
                explain: explain
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
            IModel model = enforcer.Model;
            Assertion policyAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicySection, policyType);
            return new EnforceContext
            (
                requestTokens: model.GetRequiredAssertion(PermConstants.Section.RequestSection, requestType).Tokens,
                policyTokens: policyAssertion.Tokens,
                policies: policyAssertion.Policy,
                effect: model.GetRequiredAssertion(PermConstants.Section.PolicyEffectSection, effectType).Value,
                matcher: model.GetRequiredAssertion(PermConstants.Section.MatcherSection, matcherType).Value,
                explain: explain
            );
        }

        public static EnforceContext CreatWithMatcher(IEnforcer enforcer, string matcher, bool explain)
        {
            return CreatWithMatcher(
                enforcer,
                matcher,
                PermConstants.DefaultRequestType,
                PermConstants.DefaultPolicyType,
                PermConstants.DefaultPolicyEffectType,
                explain);
        }

        public static EnforceContext CreatWithMatcher(
            IEnforcer enforcer,
            string matcher,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            bool explain = false)
        {
            IModel model = enforcer.Model;
            Assertion policyAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicySection, policyType);
            return new EnforceContext
            (
                requestTokens: model.GetRequiredAssertion(PermConstants.Section.RequestSection, requestType).Tokens,
                policyTokens: policyAssertion.Tokens,
                policies: policyAssertion.Policy,
                effect: model.GetRequiredAssertion(PermConstants.Section.PolicyEffectSection, effectType).Value,
                matcher: Util.Utility.EscapeAssertion(matcher),
                explain: explain
            );
        }
    }
}
