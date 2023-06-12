namespace Casbin.Model;

public class MatcherAssertion : Assertion
{
    public MatcherAssertion() => Section = PermConstants.Section.MatcherSection;
}
