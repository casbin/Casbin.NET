using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Casbin.Config;
using Casbin.Rbac;
using Casbin.Util;

namespace Casbin.Model
{
    public class DefaultModel : IModel
    {
        private static readonly IDictionary<string, string> _sectionNameMap = new Dictionary<string, string>() {
            { PermConstants.Section.RequestSection, PermConstants.Section.RequestSectionName},
            { PermConstants.Section.PolicySection, PermConstants.Section.PolicySectionName},
            { PermConstants.Section.RoleSection, PermConstants.Section.RoleSectionName},
            { PermConstants.Section.PolicyEffectSection, PermConstants.Section.PolicyEffectSectionName},
            { PermConstants.Section.MatcherSection, PermConstants.Section.MatcherSectionName}
        };


        private DefaultModel(IPolicyManager policyManager)
        {
            PolicyManager = policyManager;
        }

        public IPolicyManager PolicyManager { get; set; }

        public Dictionary<string, Dictionary<string, Assertion>> Sections
            => PolicyManager.Policy.Sections;

        private string _modelPath;
        public string ModelPath
        {
            get => _modelPath;
        }

        /// <summary>
        /// Creates a default model.
        /// </summary>
        /// <returns></returns>
        public static IModel Create()
        {
            return new DefaultModel(DefaultPolicyManager.Create());
        }

        /// <summary>
        /// Creates a default model from file.
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

            var model = Create();
            model.LoadModelFromFile(path);
            return model;
        }

        /// <summary>
        /// Creates a default model from text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IModel CreateFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var model = Create();
            model.LoadModelFromText(text);
            return model;
        }

        public IModel SetPolicyManager(IPolicyManager policyManager)
        {
            PolicyManager = policyManager;
            return this;
        }

        public void LoadModelFromFile(string path)
        {
            LoadModel(DefaultConfig.CreateFromFile(path));
            _modelPath = path;
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

            var assertion = new Assertion
            {
                Key = key,
                Value = value
            };

            if (section.Equals(PermConstants.Section.RequestSection)
                || section.Equals(PermConstants.Section.PolicySection))
            {
                string[] tokens = assertion.Value.Split(PermConstants.PolicySeparatorChar)
                    .Select(t => t.Trim()).ToArray();

                if (tokens.Length != 0)
                {
                    assertion.Tokens = new Dictionary<string, int>();
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        assertion.Tokens.Add($"{key}_{tokens[i]}", i);
                    }
                }
            }
            else
            {
                assertion.Value = Utility.RemoveComments(Utility.EscapeAssertion(assertion.Value));
            }

            if (Sections.ContainsKey(section) is false)
            {
                var assertionMap = new Dictionary<string, Assertion>
                {
                    [key] = assertion
                };
                Sections.Add(section, assertionMap);
            }
            else
            {
                Sections[section].Add(key, assertion);
            }

            return true;
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
            string sectionName = _sectionNameMap[section];
            string value = config.GetString($"{sectionName}::{key}");
            return AddDef(section, key, value);
        }

        private static string GetKeySuffix(int i)
        {
            return i == 1 ? string.Empty : i.ToString();
        }

        #region IPolicy

        public void BuildIncrementalRoleLink(IRoleManager roleManager, PolicyOperation policyOperation, string section,
            string policyType, IEnumerable<string> rule)
            => PolicyManager.Policy.BuildIncrementalRoleLink(roleManager, policyOperation, section, policyType, rule);

        public void BuildIncrementalRoleLinks(IRoleManager roleManager, PolicyOperation policyOperation, string section,
            string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.Policy.BuildIncrementalRoleLinks(roleManager, policyOperation, section, policyType, rules);

        public void BuildRoleLinks(IRoleManager roleManager) => PolicyManager.Policy.BuildRoleLinks(roleManager);

        public void RefreshPolicyStringSet() => PolicyManager.Policy.RefreshPolicyStringSet();

        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType)
            => PolicyManager.GetPolicy(section, policyType);

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
            => PolicyManager.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues);

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex)
            => PolicyManager.GetValuesForFieldInPolicy(section, policyType, fieldIndex);

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex)
            => PolicyManager.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule)
            => PolicyManager.HasPolicy(section, policyType, rule);

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.HasPolicies(section, policyType, rules);

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule)
            => PolicyManager.AddPolicy(section, policyType, rule);

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.AddPolicies(section, policyType, rules);

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule)
            => PolicyManager.RemovePolicy(section, policyType, rule);

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
            => PolicyManager.RemovePolicies(section, policyType, rules);

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
            => PolicyManager.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

        public void ClearPolicy() => PolicyManager.ClearPolicy();

        #endregion
    }
}
