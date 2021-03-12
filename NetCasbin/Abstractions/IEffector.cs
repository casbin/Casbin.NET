using Casbin.Effect;

namespace Casbin
{
    public interface IEffector
    {
        public bool Result { get; }

        public bool CanChain { get; }

        public bool HitPolicy { get; }

        public string EffectExpression { get; }

        public EffectExpressionType PolicyEffectType { get; }

        public void StartChain(string policyEffect);

        public bool Chain(PolicyEffect effect);

        public bool TryChain(PolicyEffect effect);

        public bool TryChain(PolicyEffect effect, out bool? result);
    }
}
