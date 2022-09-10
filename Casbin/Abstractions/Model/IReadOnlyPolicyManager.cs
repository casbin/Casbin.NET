using System.Collections.Generic;

namespace Casbin.Model;

public interface IReadOnlyPolicyManager
{
    public PolicyScanner Scan();

    public IEnumerable<IPolicyValues> GetPolicy();

    public IEnumerable<IPolicyValues> GetFilteredPolicy(int fieldIndex, IPolicyValues fieldValues);

    public IEnumerable<string> GetValuesForFieldInPolicy(int fieldIndex);

    public bool HasPolicy(IPolicyValues values);

    public bool HasPolicies(IReadOnlyList<IPolicyValues> valueList);

    public bool HasAllPolicies(IReadOnlyList<IPolicyValues> values);
}
