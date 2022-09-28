namespace Casbin.Persist;

public static class PolicyOperationExtension
{
    public static bool IsBatch(this PolicyOperation operation) =>
        operation is PolicyOperation.AddPolicies or
            PolicyOperation.RemovePolicies or
            PolicyOperation.UpdatePolicies or
            PolicyOperation.RemoveFilteredPolicy;

    public static bool IsFilter(this PolicyOperation operation) => operation is PolicyOperation.RemoveFilteredPolicy;

    public static bool IsEpoch(this PolicyOperation operation) => operation is PolicyOperation.SavePolicy;
}
