using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Persist;
using Casbin.Rbac;
using Casbin.Util;

namespace Casbin.Model
{
    public static class ModelExtension
    {
        public static bool SortPolicy(this IModel model)
        {
            string value = model.Sections.GetValue(PermConstants.Section.PolicyEffectSection,
                PermConstants.DefaultPolicyEffectType);
            switch (value)
            {
                case PermConstants.PolicyEffect.Priority:
                case PermConstants.PolicyEffect.PriorityDenyOverride:
                    return model.PolicyStoreHolder.PolicyStore.SortPolicyByPriority(PermConstants.Section.PolicySection,
                        PermConstants.DefaultPolicyType);
                case PermConstants.PolicyEffect.SubjectPriority:
                    Dictionary<string, int> map = model.Sections.GetRoleAssertion(PermConstants.DefaultRoleType)
                        .GetSubjectHierarchyMap();
                    return model.PolicyStoreHolder.PolicyStore.SortPolicyBySubjectHierarchy(
                        PermConstants.Section.PolicySection,
                        PermConstants.DefaultPolicyType, map);
                default:
                    return false;
            }
        }

        public static bool LoadPolicy(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                return false;
            }

            DefaultPolicyStore policyStore = new();
            foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
            {
                policyStore.AddNode(PermConstants.Section.PolicySection, pair.Key, pair.Value);
            }

            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                {
                    policyStore.AddNode(PermConstants.Section.RoleSection, pair.Key, pair.Value);
                }
            }

            model.AdapterHolder.EpochAdapter.LoadPolicy(policyStore);
            model.PolicyStoreHolder.PolicyStore = policyStore;
            return true;
        }

        public static async Task<bool> LoadPolicyAsync(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                return false;
            }

            DefaultPolicyStore policyStore = new();
            foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
            {
                policyStore.AddNode(PermConstants.Section.PolicySection, pair.Key, pair.Value);
            }

            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                {
                    policyStore.AddNode(PermConstants.Section.RoleSection, pair.Key, pair.Value);
                }
            }

            await model.AdapterHolder.EpochAdapter.LoadPolicyAsync(policyStore);
            model.PolicyStoreHolder.PolicyStore = policyStore;
            return true;
        }

        public static bool LoadFilteredPolicy(this IModel model, IPolicyFilter filter)
        {
            if (model.AdapterHolder.FilteredAdapter is null)
            {
                return false;
            }

            model.AdapterHolder.FilteredAdapter.LoadFilteredPolicy(model.PolicyStoreHolder.PolicyStore, filter);
            return true;
        }

        public static async Task<bool> LoadFilteredPolicyAsync(this IModel model, IPolicyFilter filter)
        {
            if (model.AdapterHolder.FilteredAdapter is null)
            {
                return false;
            }

            await model.AdapterHolder.FilteredAdapter.LoadFilteredPolicyAsync(model.PolicyStoreHolder.PolicyStore,
                filter);
            return true;
        }

        public static bool SavePolicy(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                throw new InvalidOperationException("Cannot save policy when use a readonly adapter");
            }

            if (model.AdapterHolder.FilteredAdapter is not null && model.AdapterHolder.FilteredAdapter.IsFiltered)
            {
                throw new InvalidOperationException("Cannot save filtered policies");
            }

            model.AdapterHolder.EpochAdapter.SavePolicy(model.PolicyStoreHolder.PolicyStore);
            return true;
        }

        public static async Task<bool> SavePolicyAsync(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                throw new InvalidOperationException("Cannot save policy when use a readonly adapter");
            }

            if (model.AdapterHolder.FilteredAdapter is not null && model.AdapterHolder.FilteredAdapter.IsFiltered)
            {
                throw new InvalidOperationException("Cannot save filtered policies");
            }

            await model.AdapterHolder.EpochAdapter.SavePolicyAsync(model.PolicyStoreHolder.PolicyStore);
            return true;
        }

        public static void EnableAutoSave(this IModel model, bool autoSave)
        {
            foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
            {
                pair.Value.PolicyManager.AutoSave = autoSave;
            }
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="rule"></param>
        public static void BuildIncrementalRoleLink(this IModel model, PolicyOperation policyOperation,
            string section, string roleType, IPolicyValues rule)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLink(policyOperation, rule);
            model.GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        public static void BuildIncrementalRoleLink(this IModel model, PolicyOperation policyOperation,
            string section, string roleType, IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLink(policyOperation, oldRule, newRule);
            model.GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="rules"></param>
        public static void BuildIncrementalRoleLinks(this IModel model, PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<IPolicyValues> rules)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLinks(policyOperation, rules);
            model.GFunctionCachePool.Clear(roleType);
        }

        public static void BuildIncrementalRoleLinks(this IModel model, PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<IPolicyValues> oldRules, IEnumerable<IPolicyValues> newRules)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLinks(policyOperation, oldRules, newRules);
            model.GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Initializes the roles in RBAC.
        /// </summary>
        public static void BuildRoleLinks(this IModel model, string roleType = null)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            if (roleType is not null)
            {
                RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
                assertion.RoleManager.Clear();
                assertion.BuildRoleLinks();
                model.GFunctionCachePool.Clear(roleType);
                return;
            }

            foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
            {
                string name = pair.Key;
                RoleAssertion assertion = pair.Value;
                assertion.RoleManager.Clear();
                model.GFunctionCachePool.Clear(name);
            }

            foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
            {
                RoleAssertion assertion = pair.Value;
                assertion.BuildRoleLinks();
            }
        }

        internal static IPolicyManager GetPolicyManger(this IModel model, string section,
            string policyType = PermConstants.DefaultPolicyType) =>
            model.Sections.GetPolicyAssertion(section, policyType).PolicyManager;

        public static void SetRoleManager(this IModel model, string roleType, IRoleManager roleManager)
        {
            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.RoleManager = roleManager;
            model.EnforceCache.Clear();
            model.GFunctionCachePool.Clear(roleType);
            model.ExpressionHandler.SetFunction(roleType, BuiltInFunctions.GenerateGFunction(
                assertion.RoleManager, model.GFunctionCachePool.GetCache(roleType)));
        }

        internal static IRoleManager
            GetRoleManger(this IModel model, string roleType = PermConstants.DefaultRoleType) =>
            model.Sections.GetRoleAssertion(roleType).RoleManager;
    }
}
