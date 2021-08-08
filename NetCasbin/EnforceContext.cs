using System.Collections.Generic;
using Casbin.Model;
using Casbin.Util;

namespace Casbin
{
    public readonly struct EnforceContext
    {
        public EnforceContext(
            Assertion requestAssertion, Assertion policyAssertion,
            IReadOnlyList<IReadOnlyList<string>> policies,
            string effect, string matcher,
            bool hasEval, bool explain)
        {
            RequestAssertion = requestAssertion;
            PolicyAssertion = policyAssertion;
            Effect = effect;
            Matcher = matcher;
            HasEval = hasEval;
            Explain = explain;
            Explanations = explain ? new List<IEnumerable<string>>() : null;
        }

        public IReadOnlyAssertion RequestAssertion { get; }
        public IReadOnlyAssertion PolicyAssertion { get; }
        public string Effect { get; }
        public string Matcher { get; }
        public bool HasEval { get; }
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
            string matcher = model.GetRequiredAssertion(PermConstants.Section.MatcherSection, matcherType).Value;
            bool hasEval = StringUtil.HasEval(matcher);
            Assertion requestAssertion = model.GetRequiredAssertion(PermConstants.Section.RequestSection, requestType);
            Assertion policyAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicySection, policyType);
            return new EnforceContext
            (
                requestAssertion: requestAssertion,
                policyAssertion: policyAssertion,
                policies: policyAssertion.Policy,
                effect: model.GetRequiredAssertion(PermConstants.Section.PolicyEffectSection, effectType).Value,
                matcher: matcher,
                hasEval: hasEval,
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
            matcher = StringUtil.EscapeAssertion(matcher);
            bool hasEval = StringUtil.HasEval(matcher);
            Assertion requestAssertion = model.GetRequiredAssertion(PermConstants.Section.RequestSection, requestType);
            Assertion policyAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicySection, policyType);
            return new EnforceContext
            (
                requestAssertion: requestAssertion,
                policyAssertion: policyAssertion,
                policies: policyAssertion.Policy,
                effect: model.GetRequiredAssertion(PermConstants.Section.PolicyEffectSection, effectType).Value,
                matcher: matcher,
                hasEval: hasEval,
                explain: explain
            );
        }
    }
}
