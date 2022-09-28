using System;
using System.Threading.Tasks;

namespace Casbin.Persist;

public static class WatcherExExtension
{
    internal static void NotifyWatcherEx(this IWatcherEx watcherEx, PolicyChangedMessage message)
    {
        switch (message.Operation)
        {
            case PolicyOperation.AddPolicy:
                watcherEx.UpdateForAddPolicy(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.RemovePolicy:
                watcherEx.UpdateForRemovePolicy(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.UpdatePolicy:
                watcherEx.UpdateForUpdatePolicy(message.Section, message.PolicyType, message.Values, message.NewValues);
                break;
            case PolicyOperation.SavePolicy:
                watcherEx.UpdateForSavePolicy();
                break;
            case PolicyOperation.AddPolicies:
                watcherEx.UpdateForAddPolicies(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.RemovePolicies:
                watcherEx.UpdateForRemovePolicies(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.UpdatePolicies:
                watcherEx.UpdateForUpdatePolicies(message.Section, message.PolicyType, message.ValuesList,
                    message.NewValuesList);
                break;
            case PolicyOperation.RemoveFilteredPolicy:
                watcherEx.UpdateForRemoveFilteredPolicy(message.Section, message.PolicyType, message.FieldIndex,
                    message.Values);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal static async Task NotifyWatcherExAsync(this IWatcherEx watcherEx, PolicyChangedMessage message)
    {
        switch (message.Operation)
        {
            case PolicyOperation.AddPolicy:
                await watcherEx.UpdateForAddPolicyAsync(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.RemovePolicy:
                await watcherEx.UpdateForRemovePolicyAsync(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.UpdatePolicy:
                await watcherEx.UpdateForUpdatePolicyAsync(message.Section, message.PolicyType, message.Values,
                    message.NewValues);
                break;
            case PolicyOperation.SavePolicy:
                await watcherEx.UpdateForSavePolicyAsync();
                break;
            case PolicyOperation.AddPolicies:
                await watcherEx.UpdateForAddPoliciesAsync(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.RemovePolicies:
                await watcherEx.UpdateForRemovePoliciesAsync(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.UpdatePolicies:
                await watcherEx.UpdateForUpdatePoliciesAsync(message.Section, message.PolicyType, message.ValuesList,
                    message.NewValuesList);
                break;
            case PolicyOperation.RemoveFilteredPolicy:
                await watcherEx.UpdateForRemoveFilteredPolicyAsync(message.Section, message.PolicyType,
                    message.FieldIndex, message.Values);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
