using NetCasbin.Effect;

namespace NetCasbin.Abstractions
{
    public interface IChainEffector
    {
        public bool Result { get; }

        public bool CanChain { get; }

        public bool HitPolicy { get; }

        public int HitPolicyCount { get; }

        public string EffectExpression { get; }

        public PolicyEffectType PolicyEffectType { get; }

        public void StartChain(string policyEffect);

        public bool Chain(Effect.Effect effect);

        public bool TryChain(Effect.Effect effect);

        public bool TryChain(Effect.Effect effect, out bool? result);
    }
}
