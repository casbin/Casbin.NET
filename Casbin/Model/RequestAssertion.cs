namespace Casbin.Model;

public class RequestAssertion : Assertion
{
    public RequestAssertion() => Section = PermConstants.Section.RequestSection;
}
