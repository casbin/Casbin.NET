using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicExpresso;
using NetCasbin.Effect;
using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Rbac;
using NetCasbin.Util;

namespace NetCasbin
{
    /// <summary>
    /// CoreEnforcer defines the core functionality of an enforcer.
    /// </summary>
    public class CoreEnforcer
    {
        private IEffector _effector;
        private bool _enabled;

        protected string modelPath;
        protected Model.Model model;
        protected FunctionMap functionMap;

        protected IAdapter adapter;
        protected IWatcher watcher;
        protected IRoleManager roleManager;
        protected bool autoSave;
        protected bool autoBuildRoleLinks;
        protected readonly Dictionary<string, Lambda> matcherMap = new Dictionary<string, Lambda>();

        protected void Initialize()
        {
            roleManager = new DefaultRoleManager(10);
            _effector = new DefaultEffector();
            watcher = null;
            _enabled = true;
            autoSave = true;
            autoBuildRoleLinks = true;
        }

        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <returns></returns>
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
            functionMap = FunctionMap.LoadFunctionMap();
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
            functionMap = FunctionMap.LoadFunctionMap();
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
        /// <param name="rvals">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(params object[] rvals)
        {
            if (!_enabled)
            {
                return true;
            }

            string effect = model.Model[PermConstants.Section.PolicyEffeftSection][PermConstants.DefaultPolicyEffeftType].Value;
            var rTokens = model.Model[PermConstants.Section.RequestSection][PermConstants.DefautRequestType]?.Tokens;
            var rTokensLen = rTokens?.Count();
            int policyLen = model.Model[PermConstants.Section.PolicySection][PermConstants.DefautPolicyType].Policy.Count;
            Effect.Effect[] policyEffects;
            float[] matcherResults;
            object result = null;

            string expString = model.Model[PermConstants.Section.MatcherSection][PermConstants.DefaultMatcherType].Value;
            Lambda expression = null;
            if (matcherMap.ContainsKey(expString))
            {
                expression = matcherMap[expString];
            }
            else
            {
                expression = GetAndInitializeExpression(rvals);
                matcherMap[expString] = expression;
            }

            if (policyLen != 0)
            {
                policyEffects = new Effect.Effect[policyLen];
                matcherResults = new float[policyLen];
                for (int i = 0; i < policyLen; i++)
                {
                    var pvals = model.Model[PermConstants.Section.PolicySection][PermConstants.DefautPolicyType].Policy[i];
                    if (rTokensLen != rvals.Length)
                    {
                        throw new Exception($"invalid request size: expected {rTokensLen}, got {rvals.Length}, rvals: ${rvals}");
                    }
                    var parameters = GetParameters(rvals, pvals);
                    result = expression.Invoke(parameters);
                    if (result is bool)
                    {
                        if (!(bool)result)
                        {
                            policyEffects[i] = Effect.Effect.Indeterminate;
                            continue;
                        }
                    }
                    else if (result is float)
                    {
                        if ((float)result == 0)
                        {
                            policyEffects[i] = Effect.Effect.Indeterminate;
                            continue;
                        }
                        else
                        {
                            matcherResults[i] = (float)result;
                        }
                    }
                    else
                    {
                        throw new Exception("matcher result should be bool, int or float");
                    }

                    if (parameters.Any(x => x.Name == "p_eft"))
                    {
                        string policyEft = parameters.FirstOrDefault(x => x.Name == "p_eft")?.Value as string;
                        switch (policyEft)
                        {
                            case "allow":
                                policyEffects[i] = Effect.Effect.Allow;
                                break;
                            case "deny":
                                policyEffects[i] = Effect.Effect.Deny;
                                break;
                            default:
                                policyEffects[i] = Effect.Effect.Indeterminate;
                                break;
                        }
                    }
                    else
                    {
                        policyEffects[i] = Effect.Effect.Allow;
                    }

                    if (effect.Equals(PermConstants.PolicyEffeft.Priority))
                    {
                        break;
                    }
                }
            }
            else
            {
                policyEffects = new Effect.Effect[1];
                matcherResults = new float[1];
                result = expression.Invoke(GetParameters(rvals));
                if ((bool)result)
                {
                    policyEffects[0] = Effect.Effect.Allow;
                }
                else
                {
                    policyEffects[0] = Effect.Effect.Indeterminate;
                }
            }
            result = _effector.MergeEffects(effect, policyEffects, matcherResults);
            return (bool)result;
        }

        private Lambda GetAndInitializeExpression(object[] rvals)
        {
            string expString = model.Model[PermConstants.Section.MatcherSection][PermConstants.DefaultMatcherType].Value;
            var parameters = GetParameters(rvals);
            var interpreter = GetAndInitializeInterpreter();
            var parsedExpression = interpreter.Parse(expString, parameters);
            return parsedExpression;
        }

        private Interpreter GetAndInitializeInterpreter()
        {
            var functions = new Dictionary<string, AbstractFunction>();
            foreach (var entry in functionMap.FunctionDict)
            {
                string key = entry.Key;
                var function = entry.Value;
                functions.Add(key, function);
            }

            if (model.Model.ContainsKey(PermConstants.Section.RoleSection))
            {
                foreach (var entry in model.Model[PermConstants.Section.RoleSection])
                {
                    string key = entry.Key;
                    var ast = entry.Value;
                    var rm = ast.RoleManager;
                    functions.Add(key, BuiltInFunctions.GenerateGFunction(key, rm));
                }
            }

            var interpreter = new Interpreter();
            foreach (var func in functions)
            {
                interpreter.SetFunction(func.Key, func.Value);
            }
            return interpreter;
        }

        private Parameter[] GetParameters(object[] rvals, IEnumerable<string> pvals = null)
        {
            var rTokens = model.Model[PermConstants.Section.RequestSection][PermConstants.DefautRequestType]?.Tokens;
            var rTokensLen = rTokens?.Count();
            var parameters = new Dictionary<string, object>();
            for (int i = 0; i < rTokensLen; i++)
            {
                string token = rTokens[i];
                parameters.Add(token, rvals[i]);
            }
            for (int i = 0,
                length = model.Model[PermConstants.Section.PolicySection]
                                    [PermConstants.DefautPolicyType]
                                    .Tokens.Length;
                i < length; i++)
            {
                string token = model.Model[PermConstants.Section.PolicySection][PermConstants.DefautPolicyType].Tokens[i];
                if (pvals == null)
                {
                    parameters.Add(token, string.Empty);
                }
                else
                {
                    parameters.Add(token, pvals.ElementAt(i));
                }
            }
            var result = parameters.Select(x => new Parameter(x.Key, x.Value)).ToArray();
            return result;
        }
    }
}
