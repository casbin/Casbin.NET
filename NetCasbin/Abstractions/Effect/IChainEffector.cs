namespace Casbin.Effect
{
    public interface IChainEffector
    {
        public IEffectChain CreateChain(string policyEffect);

        public IEffectChain CreateChain(string policyEffect, EffectExpressionType effectExpressionType);
    }
}
