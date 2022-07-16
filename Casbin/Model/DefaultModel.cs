using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Casbin.Caching;
using Casbin.Config;
using Casbin.Evaluation;
using Casbin.Rbac;
using Casbin.Util;

namespace Casbin.Model
{
    public class DefaultModel : IModel
    {
        private static readonly IDictionary<string, string> s_sectionNameMap = new Dictionary<string, string>
        {
            { PermConstants.Section.RequestSection, PermConstants.Section.RequestSectionName },
            { PermConstants.Section.PolicySection, PermConstants.Section.PolicySectionName },
            { PermConstants.Section.RoleSection, PermConstants.Section.RoleSectionName },
            { PermConstants.Section.PolicyEffectSection, PermConstants.Section.PolicyEffectSectionName },
            { PermConstants.Section.MatcherSection, PermConstants.Section.MatcherSectionName }
        };

        internal DefaultModel(IPolicyManager policyManager)
        {
            PolicyManager = policyManager ?? throw new NullReferenceException(nameof(policyManager));
        }

        public IGFunctionCachePool GFunctionCachePool { get; } = new GFunctionCachePool();

        public IPolicyManager PolicyManager { get; set; }

        public Dictionary<string, Dictionary<string, Assertion>> Sections
            => PolicyManager.PolicyStore.Sections;

        public bool IsSynchronized => PolicyManager.IsSynchronized;

        public string ModelPath { get; private set; }

        public IEnforceViewCache EnforceViewCache { get; } = new EnforceViewCache();

        public IExpressionHandler ExpressionHandler { get; } = new ExpressionHandler();

        public void LoadModelFromFile(string path)
        {
            LoadModel(DefaultConfig.CreateFromFile(path));
            ModelPath = path;
        }

        public void LoadModelFromText(string text)
        {
            LoadModel(DefaultConfig.CreateFromText(text));
        }

        public bool AddDef(string section, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            Assertion assertion = new() { Key = key, Value = value };

            if (section.Equals(PermConstants.Section.RequestSection)
                || section.Equals(PermConstants.Section.PolicySection))
            {
                string[] tokens = assertion.Value.Split(PermConstants.PolicySeparatorChar)
                    .Select(t => t.Trim()).ToArray();

                if (tokens.Length != 0)
                {
                    var tokenDic = new Dictionary<string, int>();
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        tokenDic.Add(tokens[i], i);
                    }

                    assertion.Tokens = tokenDic;
                }
            }
            else
            {
                // ReSharper disable once InvokeAsExtensionMethod
                assertion.Value = StringUtil.RemoveComments(assertion.Value);
            }

            if (Sections.ContainsKey(section) is false)
            {
                Dictionary<string, Assertion> assertionMap = new() { [key] = assertion };
                Sections.Add(section, assertionMap);
            }
            else
            {
                Sections[section].Add(key, assertion);
            }

            if (section.Equals(PermConstants.Section.RoleSection))
            {
                ExpressionHandler.SetFunction(key, BuiltInFunctions.GenerateGFunction(
                    assertion.RoleManager, GFunctionCachePool.GetCache(key)));
            }

            return true;
        }

        public void SetRoleManager(string roleType, IRoleManager roleManager)
        {
            Assertion assertion = GetRequiredAssertion(PermConstants.Section.RoleSection, roleType);
            assertion.RoleManager = roleManager;

            GFunctionCachePool.Clear(roleType);
            ExpressionHandler.SetFunction(roleType, BuiltInFunctions.GenerateGFunction(
                assertion.RoleManager, GFunctionCachePool.GetCache(roleType)));
        }

        /// <summary>
        /// Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="rule"></param>
        public void BuildIncrementalRoleLink(PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<string> rule)
        {
            if (Sections.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            Assertion assertion = GetRequiredAssertion(section, roleType);
            assertion.BuildIncrementalRoleLink(policyOperation, rule);

            GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        public void BuildIncrementalRoleLink(PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            if (Sections.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            Assertion assertion = GetRequiredAssertion(section, roleType);
            assertion.BuildIncrementalRoleLink(policyOperation, oldRule, newRule);

            GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        /// Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="rules"></param>
        public void BuildIncrementalRoleLinks(PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<IEnumerable<string>> rules)
        {
            if (Sections.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            Assertion assertion = GetRequiredAssertion(section, roleType);
            assertion.BuildIncrementalRoleLinks(policyOperation, rules);

            GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        public void BuildIncrementalRoleLinks(PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<IEnumerable<string>> oldRules,
            IEnumerable<IEnumerable<string>> newRules)
        {
            if (Sections.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            Assertion assertion = GetRequiredAssertion(section, roleType);
            assertion.BuildIncrementalRoleLinks(policyOperation, oldRules, newRules);

            GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        /// Initializes the roles in RBAC.
        /// </summary>
        public void BuildRoleLinks(string roleType = null)
        {
            if (Sections.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            if (roleType is not null)
            {
                Assertion assertion = GetRequiredAssertion(PermConstants.Section.RoleSection, roleType);
                assertion.RoleManager.Clear();
                assertion.BuildRoleLinks();
                GFunctionCachePool.Clear(roleType);
                return;
            }

            foreach (var pair in Sections[PermConstants.Section.RoleSection])
            {
                string name = pair.Key;
                Assertion assertion = pair.Value;

                assertion.RoleManager.Clear();
                assertion.BuildRoleLinks();
                GFunctionCachePool.Clear(name);
            }

            foreach (var pair in Sections[PermConstants.Section.RoleSection])
            {
                Assertion assertion = pair.Value;
                assertion.BuildRoleLinks();
            }
        }

        public void RefreshPolicyStringSet()
        {
            foreach (Assertion assertion in Sections.Values
                         .SelectMany(pair => pair.Values))
            {
                assertion.RefreshPolicyStringSet();
            }
        }

        public void SortPoliciesByPriority()
        {
            foreach (Assertion assertion in Sections.Values
                         .SelectMany(pair => pair.Values))
            {
                assertion.TrySortPoliciesByPriority();
            }
        }

        public void SortPoliciesBySubjectHierarchy()
        {
            if (Sections.Count == 0)
            {
                return;
            }

            if (!Sections[PermConstants.DefaultPolicyEffectType][PermConstants.DefaultPolicyEffectType].Value
                    .Equals(PermConstants.PolicyEffect.SubjectPriority))
            {
                return;
            }

            Dictionary<string, int> subjectHierarchyMap =
                GetSubjectHierarchyMap(Sections[PermConstants.DefaultRoleType][PermConstants.DefaultRoleType].Policy);
            foreach (var keyValuePair in Sections[PermConstants.DefaultPolicyType])
            {
                var assertion = keyValuePair.Value;
                assertion.TrySortPoliciesBySubjectHierarchy(subjectHierarchyMap, GetNameWithDomain);
            }
        }

        /// <summary>
        ///     Creates a default model.
        /// </summary>
        /// <returns></returns>
        public static IModel Create()
        {
            DefaultModel model = new(DefaultPolicyManager.Create());
            return model;
        }

        /// <summary>
        ///     Creates a default model from file.
        /// </summary>
        /// <param name="path">The path of the model file.</param>
        /// <returns></returns>
        public static IModel CreateFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Can not find the model file.");
            }

            IModel model = Create();
            model.LoadModelFromFile(path);
            return model;
        }

        /// <summary>
        ///     Creates a default model from text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IModel CreateFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            IModel model = Create();
            model.LoadModelFromText(text);
            return model;
        }

        private string GetNameWithDomain(string domain, string name)
        {
            return domain + PermConstants.SubjectPrioritySeparatorString + name;
        }

        private Dictionary<string, int> GetSubjectHierarchyMap(IReadOnlyList<IPolicyValues> policies)
        {
            Dictionary<string, int> refer = new Dictionary<string, int>();
            Dictionary<string, int> res = new Dictionary<string, int>();
            Dictionary<string, List<string>> policyChildenMap = new Dictionary<string, List<string>>();
            foreach (IPolicyValues policy in policies)
            {
                string domain = policy.Count > 2 ? policy[2] : null;
                string child = GetNameWithDomain(domain, policy[0]);
                string parent = GetNameWithDomain(domain, policy[1]);
                if (policyChildenMap.ContainsKey(parent))
                {
                    policyChildenMap[parent].Add(child);
                }
                else
                {
                    policyChildenMap[parent] = new List<string>(new string[] { child });
                }

                refer[parent] = refer[child] = 0;
            }

            Queue<string> q = new Queue<string>();
            foreach (KeyValuePair<string, int> keyValuePair in refer)
            {
                if (keyValuePair.Value != 0) continue;
                int level = 0;
                q.Enqueue(keyValuePair.Key);
                while (q.Count > 0)
                {
                    int size = q.Count;
                    while (size-- > 0)
                    {
                        var node = q.Dequeue();
                        res[node] = level;
                        if (policyChildenMap.ContainsKey(node))
                        {
                            foreach (string child in policyChildenMap[node])
                            {
                                q.Enqueue(child);
                            }
                        }
                    }

                    level++;
                }
            }

            return res;
        }

        private void LoadModel(IConfig config)
        {
            LoadSection(config, PermConstants.Section.RequestSection);
            LoadSection(config, PermConstants.Section.PolicySection);
            LoadSection(config, PermConstants.Section.RoleSection);
            LoadSection(config, PermConstants.Section.PolicyEffectSection);
            LoadSection(config, PermConstants.Section.MatcherSection);
        }

        private void LoadSection(IConfig config, string section)
        {
            int i = 1;
            while (true)
            {
                string key = string.Concat(section, GetKeySuffix(i));
                if (!LoadAssertion(config, section, key))
                {
                    break;
                }

                i++;
            }
        }

        private bool LoadAssertion(IConfig config, string section, string key)
        {
            string sectionName = s_sectionNameMap[section];
            string value = config.GetString($"{sectionName}::{key}");
            return AddDef(section, key, value);
        }

        private static string GetKeySuffix(int i)
        {
            return i == 1 ? string.Empty : i.ToString();
        }

        #region IPolicy Store

        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType)
            => PolicyManager.GetPolicy(section, policyType);

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues)
            => PolicyManager.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues);

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex)
            => PolicyManager.GetValuesForFieldInPolicy(section, policyType, fieldIndex);

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex)
            => PolicyManager.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule)
            => PolicyManager.HasPolicy(section, policyType, rule);

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.HasPolicies(section, policyType, rules);

        public bool HasAllPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.HasAllPolicies(section, policyType, rules);

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule)
            => PolicyManager.AddPolicy(section, policyType, rule);

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.AddPolicies(section, policyType, rules);

        public bool UpdatePolicy(string section, string policyType, IEnumerable<string> oldRule,
            IEnumerable<string> newRule)
            => PolicyManager.UpdatePolicy(section, policyType, oldRule, newRule);

        public bool UpdatePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> oldRules,
            IEnumerable<IEnumerable<string>> newRules)
            => PolicyManager.UpdatePolicies(section, policyType, oldRules, newRules);

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule)
            => PolicyManager.RemovePolicy(section, policyType, rule);

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.RemovePolicies(section, policyType, rules);

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues)
            => PolicyManager.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

        public void ClearPolicy() => PolicyManager.ClearPolicy();

        public Assertion GetRequiredAssertion(string section, string type) =>
            PolicyManager.GetRequiredAssertion(section, type);

        public bool TryGetAssertion(string section, string type, out Assertion returnAssertion) =>
            PolicyManager.TryGetAssertion(section, type, out returnAssertion);

        #endregion
    }
}
