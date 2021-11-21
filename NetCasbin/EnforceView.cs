using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Util;

namespace Casbin
{
    // TODO: The .NET Standard 2.0 TFM is not support private init.
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local")]
    public class EnforceView
    {
        public string RequestType { get; set; }
        public IReadOnlyAssertion RequestAssertion { get; private set; }
        public IReadOnlyList<string> RequestTokens { get; private set; }

        public string PolicyType { get; set; }
        public IReadOnlyAssertion PolicyAssertion { get; private set; }
        public IReadOnlyList<string> PolicyTokens { get; private set; }

        public bool SupportGeneric { get; private set; }

        public bool HasEffect { get; private set; }
        public int EffectIndex { get; private set; }

        public bool HasPriority { get; private set; }
        public int PriorityIndex { get; private set; }

        public string Effect { get; private set; }
        public EffectExpressionType EffectExpressionType { get; private set; }

        public string Matcher { get; private set; }
        public string TransformedMatcher { get; private set; }

        public bool HasRequestParameter { get; private set; }
        public bool HasPolicyParameter { get; private set; }

        public bool HasEval { get; private set; }
        public IDictionary<string, int> EvalRules { get; private set; }

        public static EnforceView Create(
            IModel model,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            string matcherType = PermConstants.DefaultMatcherType)
        {
            string matcher = model.GetRequiredAssertion(PermConstants.Section.MatcherSection, matcherType).Value;
            return CreateWithMatcher(model, matcher, requestType, policyType, effectType);
        }

        public static EnforceView CreateWithMatcher(
            IModel model,
            string matcher,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType)
        {
            IReadOnlyAssertion requestAssertion = model.GetRequiredAssertion(PermConstants.Section.RequestSection, requestType);
            IReadOnlyAssertion policyAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicySection, policyType);
            IReadOnlyAssertion effectAssertion = model.GetRequiredAssertion(PermConstants.Section.PolicyEffectSection, effectType);
            EnforceView view = new()
            {
                RequestType = requestType,
                RequestAssertion = requestAssertion,
                // TODO: ToImmutableArray dot support .NET 4.5.2
                RequestTokens = requestAssertion.Tokens.Keys.ToArray(),

                PolicyType = policyType,
                PolicyAssertion = policyAssertion,
                // TODO: ToImmutableArray dot support .NET 4.5.2
                PolicyTokens = policyAssertion.Tokens.Keys.ToArray(),

                Matcher = matcher,
                HasEval = StringUtil.HasEval(matcher),
                HasRequestParameter = matcher.Contains($"{requestType}."),
                HasPolicyParameter = matcher.Contains($"{policyType}."),

                Effect = effectAssertion.Value,
                EffectExpressionType = DefaultEffector.ParseEffectExpressionType(effectAssertion.Value),

                HasEffect = policyAssertion.TryGetTokenIndex("eft", out int effectIndex),
                EffectIndex = effectIndex,

                HasPriority = policyAssertion.TryGetTokenIndex("priority", out int priorityIndex),
                PriorityIndex = priorityIndex,
            };

            if (StringUtil.TryGetEvalRuleNames(view.Matcher, out IEnumerable<string> ruleNames))
            {
                var evalRules = new Dictionary<string, int>();
                foreach (string ruleName in ruleNames)
                {
                    string ruleNameEnum = ruleName.Replace($"{view.PolicyType}.", string.Empty);
                    if (view.PolicyAssertion.Tokens.TryGetValue(ruleNameEnum, out int ruleIndex) is false)
                    {
                        throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                    }
                    evalRules[ruleName] = ruleIndex;
                }
                view.EvalRules = evalRules;
            }

            view.SupportGeneric = Request.SupportGeneric(view.RequestTokens.Count) &&
                                  Policy.SupportGeneric(view.PolicyTokens.Count);

            // TransformedMatcher will be dynamic set when has eval
            if (view.HasEval is false)
            {
                view.TransformedMatcher = TransformMatcher(view, view.Matcher);
            }

            return view;
        }

        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        [SuppressMessage("ReSharper", "UseDeconstruction")]
        public static string TransformMatcher(in EnforceView view, string matcher)
        {
            if (view.SupportGeneric is false)
            {
                foreach (KeyValuePair<string, int> tokenPair in view.RequestAssertion.Tokens)
                {
                    matcher = matcher.Replace($"{view.RequestType}.{tokenPair.Key}",
                        $"{view.RequestType}[{tokenPair.Value}]");
                }
                foreach (KeyValuePair<string, int> tokenPair in view.PolicyAssertion.Tokens)
                {
                    matcher = matcher.Replace($"{view.PolicyType}.{tokenPair.Key}",
                        $"{view.PolicyType}[{tokenPair.Value}]");
                }
                return matcher;
            }

            foreach (KeyValuePair<string, int> tokenPair in view.RequestAssertion.Tokens)
            {
                matcher = matcher.Replace($"{view.RequestType}.{tokenPair.Key}",
                    $"{view.RequestType}.Value{tokenPair.Value + 1}");
            }
            foreach (KeyValuePair<string, int> tokenPair in view.PolicyAssertion.Tokens)
            {
                matcher = matcher.Replace($"{view.PolicyType}.{tokenPair.Key}",
                    $"{view.PolicyType}.Value{tokenPair.Value + 1}");
            }
            return matcher;
        }
    }
}
