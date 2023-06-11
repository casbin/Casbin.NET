﻿using System;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Model.Holder;
using Casbin.Persist;
#if !NET452
#endif

namespace Casbin
{
    internal static partial class InternalEnforcerExtension
    {
        private static void OnPolicyChanged(IEnforcer enforcer, PolicyChangedMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            TryBuildIncrementalRoleLinks(enforcer, message);
            TryCleanEnforceCache(enforcer);
            TryNotifyPolicyChanged(enforcer, message);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, PolicyChangedMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            TryBuildIncrementalRoleLinks(enforcer, message);
            TryCleanEnforceCache(enforcer);
            await TryNotifyPolicyChangedAsync(enforcer, message);
        }

        internal static void TryBuildIncrementalRoleLinks(this IEnforcer enforcer, PolicyChangedMessage message)
        {
            if (message.Section.Equals(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            if (message.Operation.IsBatch())
            {
                if (message.NewValuesList is null)
                {
                    enforcer.Model.BuildIncrementalRoleLinks(message.Operation, message.PolicyType,
                        message.ValuesList);
                }
                else
                {
                    enforcer.Model.BuildIncrementalRoleLinks(message.Operation, message.PolicyType,
                        message.ValuesList, message.NewValuesList);
                }
            }
            else
            {
                if (message.NewValues is null)
                {
                    enforcer.Model.BuildIncrementalRoleLink(message.Operation, message.PolicyType,
                        message.Values);
                }
                else
                {
                    enforcer.Model.BuildIncrementalRoleLink(message.Operation, message.PolicyType,
                        message.Values, message.NewValues);
                }
            }
        }

        internal static void TryCleanEnforceCache(this IEnforcer enforcer)
        {
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.ClearCache();
            }
        }

        internal static void TryNotifyPolicyChanged(this IEnforcer enforcer, PolicyChangedMessage message)
        {
            // ReSharper disable once InvertIf
            if (enforcer.AutoNotifyWatcher && enforcer.Watcher is not null)
            {
                WatcherHolder holder = enforcer.Model.WatcherHolder;
                if (holder.WatcherEx is not null)
                {
                    holder.WatcherEx.NotifyWatcherEx(message);
                    return;
                }

                if (holder.IncrementalWatcher is not null)
                {
                    holder.IncrementalWatcher.Update(message);
                    return;
                }

                // ReSharper disable once InvertIf
                if (holder.FullWatcher is not null)
                {
                    holder.FullWatcher.Update();
                }
            }
        }

        internal static async Task TryNotifyPolicyChangedAsync(this IEnforcer enforcer, PolicyChangedMessage message)
        {
            if (enforcer.AutoNotifyWatcher && enforcer.Watcher is not null)
            {
                WatcherHolder holder = enforcer.Model.WatcherHolder;
                if (holder.WatcherEx is not null)
                {
                    await holder.WatcherEx.NotifyWatcherExAsync(message);
                    return;
                }

                if (holder.IncrementalWatcher is not null)
                {
                    await holder.IncrementalWatcher.UpdateAsync(message);
                    return;
                }

                if (holder.FullWatcher is not null)
                {
                    await holder.FullWatcher.UpdateAsync();
                }
            }
        }
    }
}
