﻿using System;
using System.Collections.Generic;
using System.IO;
using Casbin.Caching;
using Casbin.Config;
using Casbin.Evaluation;
using Casbin.Model.Holder;
using Casbin.Rbac;
using Casbin.Util;

namespace Casbin.Model
{
    public class DefaultModel : IModel
    {
        private DefaultModel(ISections sections) => Sections = sections;

        public string Path { get; private set; }
        public ISections Sections { get; }
        public PolicyStoreHolder PolicyStoreHolder { get; } = new();
        public AdapterHolder AdapterHolder { get; } = new();
        public IEnforceViewCache EnforceViewCache { get; set; } = new EnforceViewCache();
        public IEnforceCache EnforceCache { get; set; } = new EnforceCache(new EnforceCacheOptions());
        public IExpressionHandler ExpressionHandler { get; set; } = new ExpressionHandler();
        public IGFunctionCachePool GFunctionCachePool { get; set; } = new GFunctionCachePool();

        public void LoadModelFromFile(string path)
        {
            Path = path;
            LoadModel(DefaultConfig.CreateFromFile(path));
        }

        public void LoadModelFromText(string text)
        {
            LoadModel(DefaultConfig.CreateFromText(text));
        }

        public bool AddDef(string section, string key, string value)
        {
            bool added = Sections.AddSection(section, key, value);
            if (added is false)
            {
                return false;
            }

            if (section.Equals(PermConstants.Section.PolicySection))
            {
                PolicyAssertion assertion = Sections.GetPolicyAssertion(key);
                LoadPolicyAssertion(key, assertion);
            }

            if (section.Equals(PermConstants.Section.RoleSection))
            {
                RoleAssertion assertion = Sections.GetRoleAssertion(key);
                LoadRoleAssertion(key, assertion);
            }

            return true;
        }

        /// <summary>
        ///     Creates a default model.
        /// </summary>
        /// <returns></returns>
        public static IModel Create()
        {
            DefaultModel model = new(new DefaultSections())
            {
                PolicyStoreHolder = { PolicyStore = new DefaultPolicyStore() }
            };
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
                throw new FileNotFoundException("The model file is not found.", path);
            }

            IModel model = Create();
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

            IModel model = Create();
            model.LoadModelFromText(text);
            return model;
        }

        private void LoadModel(IConfig config)
        {
            Sections.LoadSection(config, PermConstants.Section.RequestSection);
            Sections.LoadSection(config, PermConstants.Section.PolicySection);
            Sections.LoadSection(config, PermConstants.Section.RoleSection);
            Sections.LoadSection(config, PermConstants.Section.PolicyEffectSection);
            Sections.LoadSection(config, PermConstants.Section.MatcherSection);
            foreach (KeyValuePair<string, PolicyAssertion> pair in Sections.GetPolicyAssertions(PermConstants.Section
                         .PolicySection))
            {
                LoadPolicyAssertion(pair.Key, pair.Value);
            }

            if (Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in Sections.GetRoleAssertions(PermConstants.Section
                             .RoleSection))
                {
                    LoadRoleAssertion(pair.Key, pair.Value);
                }
            }
        }

        private void LoadPolicyAssertion(string type, PolicyAssertion assertion)
        {
            PolicyStoreHolder.PolicyStore.AddNode(PermConstants.Section.PolicySection, type, assertion);
            assertion.PolicyManager = new DefaultPolicyManager(PermConstants.Section.PolicySection, type,
                PolicyStoreHolder, AdapterHolder);
        }

        private void LoadRoleAssertion(string type, RoleAssertion assertion)
        {
            PolicyStoreHolder.PolicyStore.AddNode(PermConstants.Section.RoleSection, type, assertion);
            assertion.PolicyManager = new DefaultPolicyManager(PermConstants.Section.RoleSection, type,
                PolicyStoreHolder, AdapterHolder);
            assertion.RoleManager = new DefaultRoleManager(10);
            ExpressionHandler.SetFunction(type, BuiltInFunctions.GenerateGFunction(
                assertion.RoleManager, GFunctionCachePool.GetCache(type)));
        }
    }
}
