using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Effect;
using Casbin.Evaluation;
using Casbin.Extensions;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
using Casbin.Util;
#if !NET45
using Microsoft.Extensions.Logging;
#endif
using DynamicExpresso;

namespace Casbin
{
    /// <summary>
    /// CoreEnforcer defines the core functionality of an enforcer.
    /// </summary>
    public class Enforcer : IEnforcer
    {
        public Enforcer()
        {
        }

        public Enforcer(string modelPath, string policyPath)
            : this(modelPath, new FileAdapter(policyPath))
        {
        }

        public Enforcer(string modelPath, IAdapter adapter = null)
            : this(DefaultModel.CreateFromFile(modelPath), adapter)
        {
            ModelPath = modelPath;
        }

        public Enforcer(IModel model, IAdapter adapter = null)
        {
            if (adapter is not null)
            {
                SetAdapter(adapter);
            }

            SetModel(model);
            LoadPolicy();
        }

        #region Options
        public bool Enabled { get; private set; } = true;
        public bool AutoSave { get; private set; } = true;
        public bool AutoBuildRoleLinks { get; private set; } = true;
        public bool AutoNotifyWatcher { get; private set; } = true;
        #endregion

        #region Extensions
        public IEffector Effector { get; private set; } = new DefaultEffector();
        public IModel Model { get; private set; }
        public IAdapter Adapter { get; private set; }
        public IWatcher Watcher { get; private set; }
        public IRoleManager RoleManager { get; private set; } = new DefaultRoleManager(10);
#if !NET45
        public ILogger Logger { get; set; }
#endif
        #endregion

        public string ModelPath { get; private set; }
        public bool IsFiltered => Adapter is IFilteredAdapter {IsFiltered: true};

        public IExpressionHandler ExpressionHandler { get; private set; }

        #region Set options

        /// <summary>
        /// Changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableEnforce(bool enable)
        {
            Enabled = enable;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically to the
        /// adapter when it is added or removed.
        /// </summary>
        /// <param name="autoSave"></param>
        public void EnableAutoSave(bool autoSave)
        {
            AutoSave = autoSave;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// to the adapter when it is added or removed.
        /// </summary>
        /// <param name="autoBuildRoleLinks">Whether to automatically build the role links.</param>
        public void EnableAutoBuildRoleLinks(bool autoBuildRoleLinks)
        {
            AutoBuildRoleLinks = autoBuildRoleLinks;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// notify the Watcher when it is added or removed.
        /// </summary>
        /// <param name="autoNotifyWatcher">Whether to automatically notify watcher.</param>
        public void EnableAutoNotifyWatcher(bool autoNotifyWatcher)
        {
            AutoNotifyWatcher = autoNotifyWatcher;
        }

        #endregion

        #region Set extensions

        /// <summary>
        /// Sets the current effector.
        /// </summary>
        /// <param name="effector"></param>
        public void SetEffector(IEffector effector)
        {
            Effector = effector;
        }

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="modelPath"></param>
        public void SetModel(string modelPath)
        {
            IModel model = DefaultModel.CreateFromFile(modelPath);
            SetModel(model);
            ModelPath = modelPath;
        }

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(IModel model)
        {
            Model = model;
            ExpressionHandler = new ExpressionHandler(model);
        }

        /// <summary>
        /// Sets an adapter.
        /// </summary>
        /// <param name="adapter"></param>
        public void SetAdapter(IAdapter adapter)
        {
            Adapter = adapter;
        }

        /// <summary>
        /// Sets an watcher.
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="useAsync">Whether use async update callback.</param>
        public void SetWatcher(IWatcher watcher, bool useAsync = true)
        {
            Watcher = watcher;
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
            RoleManager = roleManager;
        }

        #endregion

        /// <summary>
        /// LoadModel reloads the model from the model CONF file. Because the policy is
        /// Attached to a model, so the policy is invalidated and needs to be reloaded by
        /// calling LoadPolicy().
        /// </summary>
        public void LoadModel()
        {
            if (ModelPath is null)
            {
                return;
            }
            Model = DefaultModel.CreateFromFile(ModelPath);
        }

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        public void BuildRoleLinks()
        {
            RoleManager.Clear();
            Model.BuildRoleLinks(RoleManager);
        }

        #region Policy Management

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public void LoadPolicy()
        {
            if (Adapter is null)
            {
                return;
            }

            Model.ClearPolicy();
            Adapter.LoadPolicy(Model);
            Model.RefreshPolicyStringSet();
            if (AutoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
        }

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public async Task LoadPolicyAsync()
        {
            if (Adapter is null)
            {
                return;
            }

            Model.ClearPolicy();
            await Adapter.LoadPolicyAsync(Model);
            if (AutoBuildRoleLinks)
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
            Model.ClearPolicy();
            if (Adapter is not IFilteredAdapter filteredAdapter)
            {
                throw new NotSupportedException("Filtered policies are not supported by this adapter.");
            }

            filteredAdapter.LoadFilteredPolicy(Model, filter);

            if (AutoBuildRoleLinks)
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
            Model.ClearPolicy();
            if (Adapter is not IFilteredAdapter filteredAdapter)
            {
                throw new NotSupportedException("Filtered policies are not supported by this adapter.");
            }

            await filteredAdapter.LoadFilteredPolicyAsync(Model, filter);

            if (AutoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
            return true;
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public void SavePolicy()
        {
            if (Adapter is null)
            {
                return;
            }

            if (IsFiltered)
            {
                throw new InvalidOperationException("Cannot save a filtered policy");
            }
            Adapter.SavePolicy(Model);
            Watcher?.Update();
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public async Task SavePolicyAsync()
        {
            if (Adapter is null)
            {
                return;
            }

            if (IsFiltered)
            {
                throw new InvalidOperationException("Cannot save a filtered policy");
            }
            await Adapter.SavePolicyAsync(Model);
            if (Watcher is not null)
            {
                await Watcher.UpdateAsync();
            }
        }

        /// <summary>
        /// Clears all policy.
        /// </summary>
        public void ClearPolicy()
        {
            Model.ClearPolicy();

#if !NET45
            Logger?.LogInformation("Policy Management, Cleared all policy.");
#endif
        }

        #endregion

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public Task<bool> EnforceAsync(params object[] requestValues)
        {
            return Task.FromResult(Enforce(requestValues));
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(params object[] requestValues)
        {
            return Enforce((IReadOnlyList<object>) requestValues);
        }

#if !NET45
        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceEx(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = Enforce(requestValues, explains);
            return (result, explains);
        }

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
        public async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExAsync(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = await EnforceAsync(requestValues, explains);
            return (result, explains);
        }
#else
        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceEx(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = Enforce(requestValues, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
        public async Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExAsync(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = await EnforceAsync(requestValues, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <param name="explains"></param>
        /// <returns>Whether to allow the request.</returns>
        private Task<bool> EnforceAsync(IReadOnlyList<object> requestValues, ICollection<IEnumerable<string>> explains = null)
        {
            return Task.FromResult(Enforce(requestValues, explains));
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="explains"></param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        private bool Enforce(IReadOnlyList<object> requestValues, ICollection<IEnumerable<string>> explains = null)
        {
            if (Enabled is false)
            {
                return true;
            }

            bool explain = explains is not null;
            string effect = Model.Sections[PermConstants.Section.PolicyEffectSection][PermConstants.DefaultPolicyEffectType].Value;
            var policyList = Model.Sections[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType].Policy;
            int policyCount = Model.Sections[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType].Policy.Count;
            string expressionString = Model.Sections[PermConstants.Section.MatcherSection][PermConstants.DefaultMatcherType].Value;

            int requestTokenCount = ExpressionHandler.RequestTokens.Count;
            if (requestTokenCount != requestValues.Count)
            {
                throw new ArgumentException($"Invalid request size: expected {requestTokenCount}, got {requestValues.Count}.");
            }
            int policyTokenCount = ExpressionHandler.PolicyTokens.Count;

            ExpressionHandler.SetRequestParameters(requestValues);

            bool hasEval = Utility.HasEval(expressionString);

            bool finalResult = false;
            IChainEffector chainEffector = Effector as IChainEffector;
            bool isChainEffector = chainEffector is not null;
            if (isChainEffector)
            {
                chainEffector.StartChain(effect);

                if (policyCount is not 0)
                {
                    foreach (IReadOnlyList<string> policyValues in policyList)
                    {
                        if (policyTokenCount != policyValues.Count)
                        {
                            throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {policyValues.Count}.");
                        }

                        ExpressionHandler.SetPolicyParameters(policyValues);

                        bool expressionResult;

                        if (hasEval)
                        {
                            string expressionStringWithRule = RewriteEval(expressionString, ExpressionHandler.PolicyTokens, policyValues);
                            expressionResult = ExpressionHandler.Invoke(expressionStringWithRule, requestValues);
                        }
                        else
                        {
                            expressionResult = ExpressionHandler.Invoke(expressionString, requestValues);
                        }

                        var nowEffect = GetEffect(expressionResult);

                        if (nowEffect is not PolicyEffect.Indeterminate && ExpressionHandler.Parameters.TryGetValue("p_eft", out Parameter parameter))
                        {
                            string policyEffect = parameter.Value as string;
                            nowEffect = policyEffect switch
                            {
                                "allow" => PolicyEffect.Allow,
                                "deny" => PolicyEffect.Deny,
                                _ => PolicyEffect.Indeterminate
                            };
                        }

                        bool chainResult = chainEffector.TryChain(nowEffect);

                        if (explain && chainEffector.HitPolicy)
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
                    if (hasEval)
                    {
                        throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                    }

                    IReadOnlyList<string> policyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                    ExpressionHandler.SetPolicyParameters(policyValues);
                    var nowEffect = GetEffect(ExpressionHandler.Invoke(expressionString, requestValues));

                    if (chainEffector.TryChain(nowEffect))
                    {
                        finalResult = chainEffector.Result;
                    }

                    if (explain && chainEffector.HitPolicy)
                    {
                        explains.Add(policyValues);
                    }
                }

#if !NET45
                if (explain)
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

            int hitPolicyIndex;
            if (policyCount != 0)
            {
                PolicyEffect[] policyEffects = new PolicyEffect[policyCount];

                for (int i = 0; i < policyCount; i++)
                {
                    IReadOnlyList<string> policyValues = policyList[i];

                    if (policyTokenCount != policyValues.Count)
                    {
                        throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {policyValues.Count}.");
                    }

                    ExpressionHandler.SetPolicyParameters(policyValues);

                    bool expressionResult;

                    if (hasEval)
                    {
                        string expressionStringWithRule = RewriteEval(expressionString, ExpressionHandler.PolicyTokens, policyValues);
                        expressionResult = ExpressionHandler.Invoke(expressionStringWithRule, requestValues);
                    }
                    else
                    {
                        expressionResult = ExpressionHandler.Invoke(expressionString, requestValues);
                    }

                    var nowEffect = GetEffect(expressionResult);

                    if (nowEffect is PolicyEffect.Indeterminate)
                    {
                        policyEffects[i] = nowEffect;
                        continue;
                    }

                    if (ExpressionHandler.Parameters.TryGetValue("p_eft", out Parameter parameter))
                    {
                        string policyEffect = parameter.Value as string;
                        nowEffect = policyEffect switch
                        {
                            "allow" => PolicyEffect.Allow,
                            "deny" => PolicyEffect.Deny,
                            _ => PolicyEffect.Indeterminate
                        };
                    }

                    policyEffects[i] = nowEffect;

                    if (effect.Equals(PermConstants.PolicyEffect.Priority))
                    {
                        break;
                    }
                }

                finalResult = Effector.MergeEffects(effect, policyEffects, null, out hitPolicyIndex);
            }
            else
            {
                if (hasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                IReadOnlyList<string> policyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                ExpressionHandler.SetPolicyParameters(policyValues);
                var nowEffect = GetEffect(ExpressionHandler.Invoke(expressionString, requestValues));
                finalResult = Effector.MergeEffects(effect, new[] { nowEffect }, null, out hitPolicyIndex);
            }

            if (explain && hitPolicyIndex is not -1)
            {
                explains.Add(policyList[hitPolicyIndex]);
            }

#if !NET45
            if (explain)
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

        private static PolicyEffect GetEffect(bool expressionResult)
        {
            return expressionResult ? PolicyEffect.Allow : PolicyEffect.Indeterminate;
        }

        private static string RewriteEval(string expressionString, IDictionary<string, int> policyTokens, IReadOnlyList<string> policyValues)
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
