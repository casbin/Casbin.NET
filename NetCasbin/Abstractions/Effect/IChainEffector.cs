using Casbin.Effect;

namespace Casbin.Effect
{
    public interface IChainEffector
    {
        public IEffectChain CreateChain(string policyEffect);
    }
}
