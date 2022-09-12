namespace Casbin.Effect
{
    public interface IChainEffector : IChainEffector<EffectChain>
    {
    }

    public interface IChainEffector<out TChain> where TChain : IEffectChain
    {
        public TChain CreateChain(string policyEffect);

        public TChain CreateChain(string policyEffect, EffectExpressionType effectExpressionType);
    }
}
