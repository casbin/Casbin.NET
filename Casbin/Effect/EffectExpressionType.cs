namespace Casbin.Effect
{
    public enum EffectExpressionType
    {
        Custom,
        AllowOverride,
        AllowAndDeny,
        DenyOverride,
        Priority,
        PriorityDenyOverride,
        PriorityAllOverride
    }
}
