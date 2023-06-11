namespace Casbin.Model;

public class PolicyEffectAssertion : Assertion
{
    public PolicyEffectAssertion() => Section = PermConstants.Section.PolicyEffectSection;
}
