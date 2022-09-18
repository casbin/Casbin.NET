namespace Casbin.Effect
{
    internal interface IChainEffector
    {
        public EffectChain CreateChain(string policyEffect);

        public EffectChain CreateChain(string policyEffect, EffectExpressionType effectExpressionType);
    }
}
