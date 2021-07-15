using System.Collections.Generic;
using System.Linq;
using NetCasbin.Model;
using NetCasbin.Util;

namespace NetCasbin
{
    internal readonly struct EnforceContext
    {
        public EnforceContext(IReadOnlyDictionary<string, int> requestTokens,
            IReadOnlyDictionary<string, int> policyTokens,
            IReadOnlyList<IReadOnlyList<string>> policies,
            Assertion policyAssertion,
            string effect, string matcher,
            bool hasEval, bool explain)
        {
            RequestTokens = requestTokens;
            PolicyTokens = policyTokens;
            Policies = policies;
            PolicyAssertion = policyAssertion;
            Effect = effect;
            Matcher = matcher;
            HasEval = hasEval;
            Explain = explain;
        }

        public IReadOnlyDictionary<string, int> RequestTokens { get; }
        public IReadOnlyDictionary<string, int> PolicyTokens { get; }
        public IReadOnlyList<IReadOnlyList<string>> Policies { get; }
        public Assertion PolicyAssertion { get; }
        public string Effect { get; }
        public string Matcher { get; }
        public bool HasEval { get; }
        public bool Explain { get; }


        public static EnforceContext Create(Model.Model model, string matcher = null, bool explain = false)
        {
            matcher = matcher is not null
                ? Utility.EscapeAssertion(matcher)
                : model.Model[PermConstants.Section.MatcherSection][PermConstants.DefaultMatcherType].Value;

            Assertion policyAssertion = model.Model[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType];
            return new EnforceContext
            (
                requestTokens: model.Model[PermConstants.Section.RequestSection][PermConstants.DefaultRequestType].Tokens,
                policyTokens: policyAssertion.Tokens,
                policies: policyAssertion.Policy,
                policyAssertion: policyAssertion,
                effect: model.Model[PermConstants.Section.PolicyEffectSection][PermConstants.DefaultPolicyEffectType].Value,
                matcher: matcher,
                hasEval: Utility.HasEval(matcher),
                explain: explain
            );
        }
    }

}
