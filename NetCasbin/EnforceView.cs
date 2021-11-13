using System.Diagnostics.CodeAnalysis;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Util;

namespace Casbin
{
    // The netstandard 2.0 TFM is not support private init.
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local")]
    public class EnforceView
    {
        public IReadOnlyAssertion RequestAssertion { get; private set; }

        public IReadOnlyAssertion PolicyAssertion { get; private set; }
        public bool HasPriority { get; private set; }
        public int PriorityIndex { get; private set; }

        public string Effect { get; private set; }
        public EffectExpressionType EffectExpressionType { get; private set; }

        public string Matcher { get; private set; }
        public bool HasEval { get; private set; }

        public static EnforceView Create(
            IModel model,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            string matcherType = PermConstants.DefaultMatcherType)
        {
            string matcher = model.GetRequiredAssertion(PermConstants.Section.MatcherSection, matcherType).Value;
            return CreateWithMatcher(model, matcher, requestType, policyType, effectType, false);
        }

        public static EnforceView CreateWithMatcher(
            IModel model,
            string matcher,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            bool escapeMatcher = true)
        {
            IReadOnlyAssertion policyAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicySection, policyType);
            IReadOnlyAssertion effectAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicyEffectSection, effectType);
            var view = new EnforceView
            {
                RequestAssertion = model.GetRequiredAssertion(PermConstants.Section.RequestSection, requestType),
                PolicyAssertion = policyAssertion,
                Matcher = escapeMatcher ? StringUtil.EscapeAssertion(matcher) : matcher,
                HasEval = StringUtil.HasEval(matcher),
                Effect = effectAssertion.Value,
                EffectExpressionType = DefaultEffector.ParseEffectExpressionType(effectAssertion.Value),
                HasPriority = policyAssertion.TryGetTokenIndex("priority", out int priorityIndex),
                PriorityIndex = priorityIndex,
            };
            return view;
        }
    }
}
