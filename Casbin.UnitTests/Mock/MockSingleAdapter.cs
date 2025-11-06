using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;

namespace Casbin.UnitTests.Mock;

public class MockSingleAdapter : FileAdapter, ISingleAdapter
{
    public List<string> SavedPolicies { get; } = new();

    public MockSingleAdapter(string filePath) : base(filePath)
    {
    }

    public void AddPolicy(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"AddPolicy: {section}.{policyType} {rule.ToText()}");
    }

    public Task AddPolicyAsync(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"AddPolicyAsync: {section}.{policyType} {rule.ToText()}");
#if NET452
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    public void UpdatePolicy(string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule)
    {
        SavedPolicies.Add($"UpdatePolicy: {section}.{policyType} {oldRule.ToText()} -> {newRule.ToText()}");
    }

    public Task UpdatePolicyAsync(string section, string policyType, IPolicyValues oldRules, IPolicyValues newRules)
    {
        SavedPolicies.Add($"UpdatePolicyAsync: {section}.{policyType} {oldRules.ToText()} -> {newRules.ToText()}");
#if NET452
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    public void RemovePolicy(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"RemovePolicy: {section}.{policyType} {rule.ToText()}");
    }

    public Task RemovePolicyAsync(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"RemovePolicyAsync: {section}.{policyType} {rule.ToText()}");
#if NET452
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    public void ClearSavedPolicies()
    {
        SavedPolicies.Clear();
    }
}
