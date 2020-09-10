using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicExpresso;
using NetCasbin.Abstractions;
using NetCasbin.Effect;
using NetCasbin.Evaluation;
using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Rbac;
using NetCasbin.Util;

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
        protected IRoleManager roleManager;
        protected bool autoSave;
        protected bool autoBuildRoleLinks;
        protected bool autoNotifyWatcher;
        protected IExpressionProvider ExpressionProvider { get; private set;}

        protected void Initialize()
        {
            roleManager = new DefaultRoleManager(10);
            _effector = new DefaultEffector();
            watcher = null;

            _enabled = true;
            autoSave = true;
            autoBuildRoleLinks = true;
            autoNotifyWatcher = true;
        }

        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <returns></returns>
        [Obsolete("The method will be moved to Model class at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/52 to know more information.")]
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
        [Obsolete("The method will be moved to Model class at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/52 to know more information.")]
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
        [Obsolete("The method will be moved to Model class at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/52 to know more information.")]
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
            ExpressionProvider = new ExpressionProvider(model);
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
            this.roleManager = roleManager;
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
        /// Clears all policy.
        /// </summary>
        public void ClearPolicy()
        {
            model.ClearPolicy();
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

            model.ClearPolicy();
            adapter.LoadPolicy(model);
            model.RefreshPolicyStringSet();
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

            model.ClearPolicy();
            await adapter.LoadPolicyAsync(model);
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
            model.ClearPolicy();
            if (!IsFiltered())
            {
                throw new Exception("filtered policies are not supported by this adapter");
            }
            (adapter as IFilteredAdapter)?.LoadFilteredPolicy(model, filter);
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
            model.ClearPolicy();
            if (!IsFiltered())
            {
                throw new Exception("filtered policies are not supported by this adapter");
            }

            if (adapter is IFilteredAdapter filteredAdapter)
            {
                await filteredAdapter.LoadFilteredPolicyAsync(model, filter);
            }

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
                throw new Exception("cannot save a filtered policy");
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
                throw new Exception("cannot save a filtered policy");
            }
            await adapter.SavePolicyAsync(model);
            if (!(watcher is null))
            {
                await watcher?.UpdateAsync();
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

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        public void BuildRoleLinks()
        {
            roleManager.Clear();
            model.BuildRoleLinks(roleManager);
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
            if (!_enabled)
            {
                return true;
            }

            string effect = model.Model[PermConstants.Section.PolicyEffectSection][PermConstants.DefaultPolicyEffectType].Value;
            int policyCount = model.Model[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType].Policy.Count;
            string expressionString = model.Model[PermConstants.Section.MatcherSection][PermConstants.DefaultMatcherType].Value;

            int requestTokenCount = ExpressionProvider.RequestAssertion.TokenCount;
            int policyTokenCount = ExpressionProvider.PolicyAssertion.TokenCount;

            bool hasEval = Utility.HasEval(expressionString);

            Lambda expression = null;
            if (!hasEval)
            {
                expression = ExpressionProvider.GetExpression(expressionString, requestValues);
            }

            Effect.Effect[] policyEffects;
            float[] matchResults;

            if (policyCount != 0)
            {
                if (requestTokenCount != requestValues.Length)
                {
                    throw new ArgumentException($"Invalid request size: expected {requestTokenCount}, got {requestValues.Length}.");
                }

                policyEffects = new Effect.Effect[policyCount];
                matchResults = new float[policyCount];
                for (int i = 0; i < policyCount; i++)
                {
                    IReadOnlyList<string> policyValues = model.Model[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType].Policy[i];

                    if (policyTokenCount != policyValues.Count)
                    {
                        throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {policyCount}.");
                    }

                    if (hasEval)
                    {
                        string expressionStringWithRule = RewriteEval(expressionString, ExpressionProvider.PolicyAssertion.Tokens, policyValues);
                        expression = ExpressionProvider.GetExpression(expressionStringWithRule, requestValues);
                    }

                    IDictionary<string, Parameter> parameters = ExpressionProvider.GetParameters(requestValues, policyValues);
                    object result = expression.Invoke(parameters.Values);
                    switch (result)
                    {
                        case bool boolResult:
                        {
                            if (!boolResult)
                            {
                                policyEffects[i] = Effect.Effect.Indeterminate;
                                continue;
                            }
                            break;
                        }
                        case float floatResult when floatResult == 0:
                            policyEffects[i] = Effect.Effect.Indeterminate;
                            continue;
                        case float floatResult:
                            matchResults[i] = floatResult;
                            break;
                        default:
                            throw new ArgumentException("Matcher result should be bool, int or float.");
                    }

                    if (parameters.TryGetValue("p_eft", out Parameter parameter))
                    {
                        string policyEffect = parameter.Value as string;
                        policyEffects[i] = policyEffect switch
                        {
                            "allow" => Effect.Effect.Allow,
                            "deny" => Effect.Effect.Deny,
                            _ => Effect.Effect.Indeterminate
                        };
                    }
                    else
                    {
                        policyEffects[i] = Effect.Effect.Allow;
                    }

                    if (effect.Equals(PermConstants.PolicyEffect.Priority))
                    {
                        break;
                    }
                }
            }
            else
            {
                if (hasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                policyEffects = new Effect.Effect[1];
                matchResults = new float[1];
                IDictionary<string, Parameter> parameters = ExpressionProvider.GetParameters(requestValues);
                bool result = (bool) expression.Invoke(parameters.Values);
                if (result)
                {
                    policyEffects[0] = Effect.Effect.Allow;
                }
                else
                {
                    policyEffects[0] = Effect.Effect.Indeterminate;
                }
            }

            bool finalResult = _effector.MergeEffects(effect, policyEffects, matchResults);
            return finalResult;
        }

        private static string RewriteEval(string expressionString, IDictionary<string, int> policyTokens, IReadOnlyList<string> policyValues)
        {
            if (!Utility.TryGetEvalRuleNames(expressionString, out IEnumerable<string> ruleNames))
            {
                return expressionString;
            }

            foreach (string ruleName in ruleNames)
            {
                if (!policyTokens.ContainsKey(ruleName))
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }
                string rule = Utility.EscapeAssertion(policyValues[policyTokens[ruleName]]);
                if (rule.Contains(">") || rule.Contains("<") || rule.Contains("="))
                {
                    expressionString = Utility.ReplaceEval(expressionString, rule);
                }
                else
                {
                    expressionString = Utility.ReplaceEval(expressionString, "false");
                }
            }

            return expressionString;
        }
    }
}
