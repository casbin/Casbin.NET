using Casbin.Effect;

namespace Casbin.Model.Holder;

public class EffectorHolder
{
    private IEffector _effector;

    public IEffector Effector
    {
        get => _effector;
        set
        {
            _effector = value;
            DetermineEffector(value);
        }
    }

    public IChainEffector ChainEffector { get; private set; }

    private void DetermineEffector(IEffector effector) => ChainEffector = effector as IChainEffector;
}
