using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casbin.Effect
{
    public interface IEffectChain
    {
        public bool Result { get; }

        public bool CanChain { get; }

        public bool HitPolicy { get; }

        public int HitPolicyCount { get; }

        public string EffectExpression { get; }

        public EffectExpressionType EffectExpressionType { get; }

        public bool Chain(PolicyEffect effect);

        public bool TryChain(PolicyEffect effect);

        public bool TryChain(PolicyEffect effect, out bool? result);
    }
}
