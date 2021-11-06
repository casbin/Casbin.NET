using System.Collections.Generic;
using Casbin.Effect;

namespace Casbin
{
    internal struct EnforceSession
    {
        internal IReadOnlyList<object> RequestValues { get; set; }
        internal IReadOnlyList<string> PolicyValues { get; set; }

        internal string ExpressionString { get; set; }

        internal int PolicyIndex { get; set; }
        internal int PolicyCount { get; set; }

        internal PolicyEffect PolicyEffect { get; set; }
        internal PolicyEffect[] PolicyEffects { get; set; }

        internal bool Determined { get; private set; }
        internal bool EnforceResult { get; set; }

        internal EffectExpressionType EffectExpressionType {  get; set; }
        internal bool ExpressionResult { get; set; }

        internal bool IsChainEffector { get; set; }
        internal IEffectChain EffectChain { get; set; }

        internal bool HasPriority { get; set; }
        internal int PriorityIndex { get; set; }
        internal int? Priority { get; set; }

        internal void DetermineResult(bool result)
        {
            Determined = true;
            EnforceResult = result;
        }
    }
}
