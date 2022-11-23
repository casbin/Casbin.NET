using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetCasbin.Abstractions;
using NetCasbin.Caching;
using NetCasbin.Effect;
using NetCasbin.Evaluation;
using NetCasbin.Extensions;
using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Rbac;
using NetCasbin.Util;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace NetCasbin
{
    /// <summary>
    /// CoreEnforcer defines the core functionality of an enforcer.
    /// </summary>
    public class CoreEnforcer : ICoreEnforcer
    {
        private IEffector _effector;
        private bool _enabled;

        protected string modelPath;
        protected Model.Model model;

        protected IAdapter adapter;
        protected IWatcher watcher;
        protected bool autoSave;
        protected bool autoBuildRoleLinks;
        protected bool autoNotifyWatcher;
        protected bool autoCleanEnforceCache = true;
        internal IExpressionHandler ExpressionHandler { get; private set; }

        private bool _enableCache;
        public IEnforceCache EnforceCache { get; private set; }
#if !NET452
        public ILogger Logger { get; set; }
#endif

        protected void Initialize(EnforcerOptions options)
        {
            _effector = new DefaultEffector();
            watcher = null;

            _enabled = options.Enabled;
            autoSave = true;
            autoBuildRoleLinks = options.AutoBuildRoleLinks;
            autoNotifyWatcher = options.AutoNotifyWatcher;
        }

        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <returns></returns>
        [Obsolete(
            "The method will be moved to Model class at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/52 to know more information.")]
        public static Model.Model NewModel()
        {
            var model = new Model.Model();
            return model;
        }

        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [Obsolete(
            "The method will be moved to Model class at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/52 to know more information.")]
        public static Model.Model NewModel(string text)
        {
            var model = new Model.Model();
            model.LoadModelFromText(text);
            return model;
        }

        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <param name="modelPath">The path of the model file.</param>
        /// <param name="unused">Unused parameter, just for differentiating with  NewModel(String text).</param>
        /// <returns></returns>
        [Obsolete(
            "The method will be moved to Model class at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/52 to know more information.")]
        public static Model.Model NewModel(string modelPath, string unused)
        {
            var model = new Model.Model();
            if (!string.IsNullOrEmpty(modelPath))
            {
                model.LoadModel(modelPath);
            }

            return model;
        }

        /// <summary>
        /// LoadModel reloads the model from the model CONF file. Because the policy is
        /// Attached to a model, so the policy is invalidated and needs to be reloaded by
        /// calling LoadPolicy().
        /// </summary>
        public void LoadModel()
        {
            model = NewModel();
            model.LoadModel(modelPath);
        }

        /// <summary>
        /// Gets the current model.
        /// </summary>
        /// <returns>The model of the enforcer.</returns>
        public Model.Model GetModel() => model;

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(Model.Model model)
        {
            this.model = model;
            ExpressionHandler = new ExpressionHandler(model);
            if (autoCleanEnforceCache)
            {
                EnforceCache?.Clear();
#if !NET452
                Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }
        }

        /// <summary>
        /// Gets the current adapter.
        /// </summary>
        /// <returns></returns>
        public IAdapter GetAdapter() => adapter;

        /// <summary>
        /// Sets an adapter.
        /// </summary>
        /// <param name="adapter"></param>
        public void SetAdapter(IAdapter adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// Sets an watcher.
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="useAsync">Whether use async update callback.</param>
        public void SetWatcher(IWatcher watcher, bool useAsync = true)
        {
            this.watcher = watcher;
            if (useAsync)
            {
                watcher?.SetUpdateCallback(LoadPolicyAsync);
                return;
            }

            watcher?.SetUpdateCallback(LoadPolicy);
        }

        /// <summary>
        /// Sets the current role manager.
        /// </summary>
        /// <param name="roleManager"></param>
        public void SetRoleManager(IRoleManager roleManager)
        {
            SetRoleManager(PermConstants.DefaultRoleType, roleManager);
            ExpressionHandler.SetGFunctions();
        }

        /// <summary>
        /// Sets the current role manager.
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="roleManager"></param>
        public void SetRoleManager(string roleType, IRoleManager roleManager)
        {
            Assertion assertion = model.GetExistAssertion(PermConstants.Section.RoleSection, roleType);
            assertion.RoleManager = roleManager;
            if (autoBuildRoleLinks)
            {
                assertion.BuildRoleLinks();
            }
        }

        /// <summary>
        /// Sets the current effector.
        /// </summary>
        /// <param name="effector"></param>
        public void SetEffector(IEffector effector)
        {
            _effector = effector;
        }

        /// <summary>
        /// Sets an enforce cache.
        /// </summary>
        /// <param name="enforceCache"></param>
        public void SetEnforceCache(IEnforceCache enforceCache)
        {
            EnforceCache = enforceCache;
        }

        /// <summary>
        /// Clears all policy.
        /// </summary>
        public void ClearPolicy()
        {
            model.ClearPolicy();
            if (autoCleanEnforceCache)
            {
                EnforceCache?.Clear();
#if !NET452
                Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }
#if !NET452
            Logger?.LogInformation("Policy Management, Cleared all policy.");
#endif
        }

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public void LoadPolicy()
        {
            if (adapter is null)
            {
                return;
            }

            ClearPolicy();
            adapter.LoadPolicy(model);

            model.RefreshPolicyStringSet();
            model.SortPoliciesByPriority();

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
        }

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public async Task LoadPolicyAsync()
        {
            if (adapter is null)
            {
                return;
            }

            ClearPolicy();
            await adapter.LoadPolicyAsync(model);

            model.RefreshPolicyStringSet();
            model.SortPoliciesByPriority();

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public bool LoadFilteredPolicy(Filter filter)
        {
            ClearPolicy();
            if (adapter is not IFilteredAdapter filteredAdapter)
            {
                throw new NotSupportedException("Filtered policies are not supported by this adapter.");
            }

            filteredAdapter.LoadFilteredPolicy(model, filter);

            model.RefreshPolicyStringSet();
            model.SortPoliciesByPriority();

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public async Task<bool> LoadFilteredPolicyAsync(Filter filter)
        {
            ClearPolicy();
            if (adapter is not IFilteredAdapter filteredAdapter)
            {
                throw new NotSupportedException("Filtered policies are not supported by this adapter.");
            }

            await filteredAdapter.LoadFilteredPolicyAsync(model, filter);

            model.RefreshPolicyStringSet();
            model.SortPoliciesByPriority();

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Returns true if the loaded policy has been filtered.
        /// </summary>
        /// <returns>if the loaded policy has been filtered.</returns>
        public bool IsFiltered()
        {
            if (adapter is IFilteredAdapter filteredAdapter)
            {
                return filteredAdapter.IsFiltered;
            }

            return false;
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public void SavePolicy()
        {
            if (adapter is null)
            {
                return;
            }

            if (IsFiltered())
            {
                throw new InvalidOperationException("Cannot save a filtered policy");
            }

            adapter.SavePolicy(model);
            watcher?.Update();
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public async Task SavePolicyAsync()
        {
            if (IsFiltered())
            {
                throw new InvalidOperationException("Cannot save a filtered policy");
            }

            await adapter.SavePolicyAsync(model);
            if (watcher is not null)
            {
                await watcher.UpdateAsync();
            }
        }

        /// <summary>
        /// Changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableEnforce(bool enable)
        {
            _enabled = enable;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically to the
        /// adapter when it is added or removed.
        /// </summary>
        /// <param name="autoSave"></param>
        public void EnableAutoSave(bool autoSave)
        {
            this.autoSave = autoSave;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// to the adapter when it is added or removed.
        /// </summary>
        /// <param name="autoBuildRoleLinks">Whether to automatically build the role links.</param>
        public void EnableAutoBuildRoleLinks(bool autoBuildRoleLinks)
        {
            this.autoBuildRoleLinks = autoBuildRoleLinks;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// notify the Watcher when it is added or removed.
        /// </summary>
        /// <param name="autoNotifyWatcher">Whether to automatically notify watcher.</param>
        public void EnableAutoNotifyWatcher(bool autoNotifyWatcher)
        {
            this.autoNotifyWatcher = autoNotifyWatcher;
        }

        public void EnableCache(bool enableCache)
        {
            _enableCache = enableCache;
        }

        public void EnableAutoCleanEnforceCache(bool autoCleanEnforceCache)
        {
            this.autoCleanEnforceCache = autoCleanEnforceCache;
        }

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        public void BuildRoleLinks()
        {
            model.BuildRoleLinks();
        }

        #region Enforce

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(params object[] requestValues)
        {
            if (_enabled is false)
            {
                return true;
            }

            if (_enableCache is false)
            {
                return InternalEnforce(requestValues);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return InternalEnforce(requestValues);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
#if !NET452
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = InternalEnforce(requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceAsync(params object[] requestValues)
        {
            if (_enabled is false)
            {
                return true;
            }

            if (_enableCache is false)
            {
                return await InternalEnforceAsync(requestValues);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return await InternalEnforceAsync(requestValues);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            bool? tryGetCachedResult = await EnforceCache.TryGetResultAsync(requestValues, key);
            if (tryGetCachedResult.HasValue)
            {
                bool cachedResult = tryGetCachedResult.Value;
#if !NET452
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = await InternalEnforceAsync(requestValues);

            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool EnforceWithMatcher(string matcher, params object[] requestValues)
        {
            if (_enabled is false)
            {
                return true;
            }

            if (string.IsNullOrEmpty(matcher))
            {
                throw new ArgumentException($"'{nameof(matcher)}' cannot be null or empty.", nameof(matcher));
            }

            if (requestValues is null)
            {
                throw new ArgumentNullException(nameof(requestValues));
            }

            if (_enableCache is false)
            {
                return InternalEnforce(requestValues, matcher);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return InternalEnforce(requestValues, matcher);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
#if !NET452
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = InternalEnforce(requestValues, matcher);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceWithMatcherAsync(string matcher, params object[] requestValues)
        {
            if (_enabled is false)
            {
                return true;
            }

            if (string.IsNullOrEmpty(matcher))
            {
                throw new ArgumentException($"'{nameof(matcher)}' cannot be null or empty.", nameof(matcher));
            }

            if (requestValues is null)
            {
                throw new ArgumentNullException(nameof(requestValues));
            }

            if (_enableCache is false)
            {
                return await InternalEnforceAsync(requestValues, matcher);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return await InternalEnforceAsync(requestValues, matcher);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            bool? tryGetCachedResult = await EnforceCache.TryGetResultAsync(requestValues, key);
            if (tryGetCachedResult.HasValue)
            {
                bool cachedResult = tryGetCachedResult.Value;
#if !NET452
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = await InternalEnforceAsync(requestValues, matcher);

            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return result;
        }

        #endregion

        #region EnforceEx

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET452
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceEx(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (_enabled is false)
            {
                return (true, explains);
            }

            if (_enableCache is false)
            {
                return (InternalEnforce(requestValues, null, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (InternalEnforce(requestValues, null, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result = InternalEnforce(requestValues, null, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return (result, explains);
        }
#else
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceEx(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = InternalEnforce(requestValues, null, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET452
        public async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExAsync(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (_enabled is false)
            {
                return (true, explains);
            }

            if (_enableCache is false)
            {
                return (await InternalEnforceAsync(requestValues, null, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (await InternalEnforceAsync(requestValues, null, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result = await InternalEnforceAsync(requestValues, null, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return (result, explains);
        }
#else
        public async Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExAsync(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = await InternalEnforceAsync(requestValues, null, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET452
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceExWithMatcher(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (_enabled is false)
            {
                return (true, explains);
            }

            if (_enableCache is false)
            {
                return (InternalEnforce(requestValues, matcher, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (InternalEnforce(requestValues, matcher, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result = InternalEnforce(requestValues, matcher, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return (result, explains);
        }
#else
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceExWithMatcher(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = InternalEnforce(requestValues, matcher, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET452
        public async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExWithMatcherAsync(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (_enabled is false)
            {
                return (true, explains);
            }

            if (_enableCache is false)
            {
                return (await InternalEnforceAsync(requestValues, matcher, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (await InternalEnforceAsync(requestValues, matcher, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result = await InternalEnforceAsync(requestValues, matcher, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return (result, explains);
        }
#else
        public async Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExWithMatcherAsync(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = await InternalEnforceAsync(requestValues, matcher, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        #endregion

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="explains"></param>
        /// <returns>Whether to allow the request.</returns>
        private Task<bool> InternalEnforceAsync(IReadOnlyList<object> requestValues, string matcher = null,
            ICollection<IEnumerable<string>> explains = null)
        {
            return Task.FromResult(InternalEnforce(requestValues, matcher, explains));
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="explains">Collection of matched policy explains</param>
        /// <returns>Whether to allow the request.</returns>
        private bool InternalEnforce(IReadOnlyList<object> requestValues, string matcher = null,
            ICollection<IEnumerable<string>> explains = null)
        {
            var context = EnforceContext.Create(model, matcher, explains is not null);

            if (context.RequestTokens.Count != requestValues.Count)
            {
                throw new ArgumentException(
                    $"Invalid request size: expected {context.RequestTokens.Count}, got {requestValues.Count}.");
            }

            ExpressionHandler.SetRequest(requestValues);

            IChainEffector chainEffector = _effector as IChainEffector;
            PolicyEffectType effectType = chainEffector.PolicyEffectType;

            if (chainEffector is not null)
            {
                return InternalEnforceWithChainEffector(context, chainEffector, requestValues, explains);
            }

            if (effectType is PolicyEffectType.PriorityDenyOverride)
            {
                ThrowHelper.ThrowNotSupportException(
                    $"Only {nameof(IChainEffector)} support {nameof(PolicyEffectType.PriorityDenyOverride)} policy effect expression.");
            }

            bool finalResult = false;
            int hitPolicyIndex;
            int policyCount = context.Policies.Count;
            if (policyCount != 0)
            {
                Effect.Effect[] policyEffects = new Effect.Effect[policyCount];

                for (int i = 0; i < context.Policies.Count; i++)
                {
                    IReadOnlyList<string> policyValues = context.Policies[i];

                    if (context.PolicyTokens.Count != policyValues.Count)
                    {
                        throw new ArgumentException(
                            $"Invalid policy size: expected {context.PolicyTokens.Count}, got {policyValues.Count}.");
                    }

                    ExpressionHandler.SetPolicy(policyValues);

                    bool expressionResult;

                    if (context.HasEval)
                    {
                        string expressionStringWithRule =
                            RewriteEval(context.Matcher, ExpressionHandler.PolicyTokens, policyValues);
                        expressionResult = ExpressionHandler.Invoke(expressionStringWithRule);
                    }
                    else
                    {
                        expressionResult = ExpressionHandler.Invoke(context.Matcher);
                    }

                    var nowEffect = GetEffect(expressionResult);

                    if (nowEffect is Effect.Effect.Indeterminate)
                    {
                        policyEffects[i] = nowEffect;
                        continue;
                    }

                    if (ExpressionHandler.PolicyTokens.TryGetValue("p_eft", out int index))
                    {
                        string policyEffect = policyValues[index];
                        nowEffect = policyEffect switch
                        {
                            "allow" => Effect.Effect.Allow,
                            "deny" => Effect.Effect.Deny,
                            _ => Effect.Effect.Indeterminate
                        };
                    }

                    policyEffects[i] = nowEffect;

                    if (context.Effect.Equals(PermConstants.PolicyEffect.Priority))
                    {
                        break;
                    }
                }

                finalResult = _effector.MergeEffects(context.Effect, policyEffects, null, out hitPolicyIndex);
            }
            else
            {
                if (context.HasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                IReadOnlyList<string> policyValues =
                    Enumerable.Repeat(string.Empty, context.PolicyTokens.Count).ToArray();
                ExpressionHandler.SetPolicy(policyValues);
                Effect.Effect nowEffect = GetEffect(ExpressionHandler.Invoke(context.Matcher));
                finalResult = _effector.MergeEffects(context.Effect, new[] { nowEffect }, null, out hitPolicyIndex);
            }

            if (context.Explain && hitPolicyIndex is not -1)
            {
                explains.Add(context.Policies[hitPolicyIndex]);
            }

#if !NET452
            if (context.Explain)
            {
                Logger?.LogEnforceResult(requestValues, finalResult, explains);
            }
            else
            {
                Logger?.LogEnforceResult(requestValues, finalResult);
            }
#endif
            return finalResult;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act). Works specifically with IChainEffector
        /// </summary>
        /// <param name="context">Storage of enforcer variables</param>
        /// <param name="chainEffector"></param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, can be class instances if ABAC is used.</param>
        /// <param name="explains">Collection of matched policy explains</param>
        /// <returns>Whether to allow the request.</returns>
        private bool InternalEnforceWithChainEffector(
            EnforceContext context,
            IChainEffector chainEffector,
            IReadOnlyList<object> requestValues = null,
            ICollection<IEnumerable<string>> explains = null)
        {
            bool finalResult = false;
            chainEffector.StartChain(context.Effect);

            bool hasPriority = context.PolicyAssertion.TryGetPriorityIndex(out int priorityIndex);
            bool isPriorityDenyOverrideEfffet = chainEffector.PolicyEffectType is PolicyEffectType.PriorityDenyOverride;
            int? priority = null;
            var nowEffect = Effect.Effect.Indeterminate;

            if (context.Policies.Count is not 0)
            {
                foreach (IReadOnlyList<string> policyValues in context.Policies)
                {
                    if (context.PolicyTokens.Count != policyValues.Count)
                    {
                        throw new ArgumentException(
                            $"Invalid policy size: expected {context.PolicyTokens.Count}, got {policyValues.Count}.");
                    }

                    if (hasPriority && isPriorityDenyOverrideEfffet)
                    {
                        if (int.TryParse(policyValues[priorityIndex], out int nowPriority))
                        {
                            if (priority.HasValue && nowPriority != priority.Value
                                                  && chainEffector.HitPolicyCount > 0)
                            {
                                break;
                            }

                            priority = nowPriority;
                        }
                    }

                    ExpressionHandler.SetPolicy(policyValues);

                    bool expressionResult;

                    if (context.HasEval)
                    {
                        string expressionStringWithRule =
                            RewriteEval(context.Matcher, ExpressionHandler.PolicyTokens, policyValues);
                        expressionResult = ExpressionHandler.Invoke(expressionStringWithRule);
                    }
                    else
                    {
                        expressionResult = ExpressionHandler.Invoke(context.Matcher);
                    }

                    nowEffect = GetEffect(expressionResult);

                    if (nowEffect is not Effect.Effect.Indeterminate
                        && ExpressionHandler.PolicyTokens.TryGetValue("p_eft", out int index))
                    {
                        string policyEffect = policyValues[index];
                        nowEffect = policyEffect switch
                        {
                            "allow" => Effect.Effect.Allow,
                            "deny" => Effect.Effect.Deny,
                            _ => Effect.Effect.Indeterminate
                        };
                    }

                    bool chainResult = chainEffector.TryChain(nowEffect);

                    if (context.Explain && chainEffector.HitPolicy)
                    {
                        explains.Add(policyValues);
                    }

                    if (chainResult is false || chainEffector.CanChain is false)
                    {
                        break;
                    }
                }

                finalResult = chainEffector.Result;
            }
            else
            {
                if (context.HasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                IReadOnlyList<string> policyValues =
                    Enumerable.Repeat(string.Empty, context.PolicyTokens.Count).ToArray();
                ExpressionHandler.SetPolicy(policyValues);
                nowEffect = GetEffect(ExpressionHandler.Invoke(context.Matcher));

                if (chainEffector.TryChain(nowEffect))
                {
                    finalResult = chainEffector.Result;
                }

                if (context.Explain && chainEffector.HitPolicy)
                {
                    explains.Add(policyValues);
                }
            }

#if !NET452
            if (context.Explain)
            {
                Logger?.LogEnforceResult(requestValues, finalResult, explains);
            }
            else
            {
                Logger?.LogEnforceResult(requestValues, finalResult);
            }
#endif
            return finalResult;
        }

        private static Effect.Effect GetEffect(bool expressionResult)
        {
            return expressionResult ? Effect.Effect.Allow : Effect.Effect.Indeterminate;
        }

        private static string RewriteEval(string expressionString, IReadOnlyDictionary<string, int> policyTokens,
            IReadOnlyList<string> policyValues)
        {
            if (Utility.TryGetEvalRuleNames(expressionString, out IEnumerable<string> ruleNames) is false)
            {
                return expressionString;
            }

            Dictionary<string, string> rules = new();
            foreach (string ruleName in ruleNames)
            {
                if (policyTokens.TryGetValue(ruleName, out int ruleIndex) is false)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                rules[ruleName] = Utility.EscapeAssertion(policyValues[ruleIndex]);
            }

            expressionString = Utility.ReplaceEval(expressionString, rules);
            return expressionString;
        }
    }
}
