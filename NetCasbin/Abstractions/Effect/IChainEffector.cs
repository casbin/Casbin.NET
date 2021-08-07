namespace Casbin.Effect
{
    public interface IChainEffector
    {
        public bool Result { get; }

        public bool CanChain { get; }

        public bool HitPolicy { get; }

        public string EffectExpression { get; }

        public EffectExpressionType EffectExpressionType { get; }

        public void StartChain(string policyEffect);

        public bool Chain(PolicyEffect effect);

        public bool TryChain(PolicyEffect effect);

        public bool TryChain(PolicyEffect effect, out bool? result);
    }
}
