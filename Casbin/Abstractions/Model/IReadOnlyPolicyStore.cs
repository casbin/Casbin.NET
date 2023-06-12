using System.Collections.Generic;

namespace Casbin.Model;

public interface IReadOnlyPolicyStore
{
    public bool ContainsNodes(string section);

    public bool ContainsNode(string section, string policyType);

    public PolicyScanner Scan(string section, string policyType);

    public IEnumerable<IPolicyValues> GetPolicy(string section, string policyType);

    public IEnumerable<string> GetPolicyTypes(string section);

    public IDictionary<string, IEnumerable<string>> GetPolicyTypesAllSections();

    public IDictionary<string, IEnumerable<IPolicyValues>> GetPolicyAllType(string section);

    public IEnumerable<IPolicyValues> GetFilteredPolicy(string section, string policyType, int fieldIndex,
        IPolicyValues fieldValues);

    public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex);

    public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex);

    public bool HasPolicy(string section, string policyType, IPolicyValues values);

    public bool HasPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList);

    public bool HasAllPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList);
}
