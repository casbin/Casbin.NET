namespace Casbin.Model;

public class PolicyAssertion : Assertion
{
    public PolicyAssertion() => Section = PermConstants.Section.PolicySection;

    public IPolicyManager PolicyManager { get; internal set; }

    public PolicyScanner<TRequest> Scan<TRequest>(in TRequest request) where TRequest : IRequestValues =>
        new(PolicyManager.Scan(), request);

    public bool TryGetPriorityIndex(out int index)
    {
        if (Tokens is null)
        {
            index = -1;
            return false;
        }

        return Tokens.TryGetValue(PermConstants.Token.Priority, out index);
    }

    public bool TryGetDomainIndex(out int index)
    {
        if (Tokens is null)
        {
            index = -1;
            return false;
        }

        return Tokens.TryGetValue(PermConstants.Token.Domain, out index);
    }

    public bool TryGetSubjectIndex(out int index)
    {
        if (Tokens is null)
        {
            index = -1;
            return false;
        }

        return Tokens.TryGetValue(PermConstants.Token.Subject, out index);
    }
}
