using DynamicExpresso;
using NetCasbin.Effect;
using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Rbac;
using NetCasbin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCasbin
{
    /// <summary>
    /// CoreEnforcer defines the core functionality of an enforcer.
    /// </summary>
    public class CoreEnforcer
    {
        private IEffector eft;
        private bool _enabled;

        protected string modelPath;
        protected Model.Model model;
        protected FunctionMap fm;

        protected IAdapter adapter;
        protected IWatcher watcher;
        protected IRoleManager rm;
        protected bool autoSave;
        protected bool autoBuildRoleLinks;
        protected readonly Dictionary<string, Lambda> matcherMap = new Dictionary<string, Lambda>();

        protected void Initialize()
        {
            rm = new DefaultRoleManager(10);
            eft = new DefaultEffector();
            watcher = null;
            _enabled = true;
            autoSave = true;
            autoBuildRoleLinks = true;
        }

        /// <summary>
        /// creates a model.
        /// </summary>
        /// <returns></returns>
        public static Model.Model NewModel()
        {
            var m = new Model.Model();
            return m;
        }

        /// <summary>
        ///  creates a model.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Model.Model NewModel(string text)
        {
            var m = new Model.Model();
            m.LoadModelFromText(text);
            return m;
        }

        /// <summary>
        ///  creates a model.
        /// </summary>
        /// <param name="modelPath">the path of the model file.</param>
        /// <param name="unused">unused parameter, just for differentiating with  NewModel(String text).</param>
        /// <returns>the model.</returns>
        public static Model.Model NewModel(string modelPath, string unused)
        {
            var m = new Model.Model();
            if (!string.IsNullOrEmpty(modelPath))
            {
                m.LoadModel(modelPath);
            }
            return m;
        }

        /// <summary>
        /// LoadModel reloads the model from the model CONF file. Because the policy is
        /// attached to a model, so the policy is invalidated and needs to be reloaded by
        /// calling LoadPolicy().
        /// calling LoadPolicy().
        /// </summary>
        public void LoadModel()
        {
            model = NewModel();
            model.LoadModel(modelPath);
            fm = FunctionMap.LoadFunctionMap();
        }

        /// <summary>
        /// gets the current model.
        /// </summary>
        /// <returns>the model of the enforcer.</returns>
        public Model.Model GetModel() => model;

        /// <summary>
        /// sets the current model.
        /// </summary>
        /// <param name="model"> the model.</param>
        public void SetModel(Model.Model model)
        {
            this.model = model;
            fm = FunctionMap.LoadFunctionMap();
        }

        /// <summary>
        /// gets the current adapter.
        /// </summary>
        /// <returns></returns>
        public IAdapter GetAdapter() => adapter;

        public void SetAdapter(IAdapter adapter)
        {
            this.adapter = adapter;
        }

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
        /// SetRoleManager sets the current role manager.
        /// </summary>
        /// <param name="rm"></param>
        public void SetRoleManager(IRoleManager rm)
        {
            this.rm = rm;
        }

        /// <summary>
        ///  sets the current effector.
        /// </summary>
        /// <param name="eft"></param>
        public void SetEffector(IEffector eft)
        {
            this.eft = eft;
        }

        /// <summary>
        ///  clears all policy.
        /// </summary>
        public void ClearPolicy()
        {
            model.ClearPolicy();
        }

        /// <summary>
        ///  reloads the policy from file/database.
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
        ///  reloads the policy from file/database.
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
        ///  reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">he filter used to specify which type of policy should be loaded.</param>
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
        ///  reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">he filter used to specify which type of policy should be loaded.</param>
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
        ///  returns true if the loaded policy has been filtered.
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
        /// saves the current policy (usually after changed with Casbin API)
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
        /// saves the current policy (usually after changed with Casbin API)
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
        /// enableEnforce changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableEnforce(bool enable)
        {
            _enabled = enable;
        }

        /// <summary>
        ///  enableAutoSave controls whether to save a policy rule automatically to the
        ///   adapter when it is added or removed.
        /// </summary>
        /// <param name="autoSave"></param>
        public void EnableAutoSave(bool autoSave)
        {
            this.autoSave = autoSave;
        }

        /// <summary>
        ///  controls whether to save a policy rule automatically
        ///   to the adapter when it is added or removed.
        /// </summary>
        /// <param name="autoBuildRoleLinks">whether to automatically build the role links.</param>
        public void EnableAutoBuildRoleLinks(bool autoBuildRoleLinks)
        {
            this.autoBuildRoleLinks = autoBuildRoleLinks;
        }

        /// <summary>
        /// BuildRoleLinks manually rebuild the role inheritance relations.
        /// </summary>
        public void BuildRoleLinks()
        {
            rm.Clear();
            model.BuildRoleLinks(rm);
        }

        /// <summary>
        /// enforce decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="rvals">the request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>whether to allow the request.</returns>
        public bool Enforce(params object[] rvals)
        {
            if (!_enabled)
            {
                return true;
            }

            var effect = model.Model["e"]["e"].Value;
            var rTokens = model.Model["r"]["r"]?.Tokens;
            var rTokensLen = rTokens?.Count();
            var policyLen = model.Model["p"]["p"].Policy.Count;
            Effect.Effect[] policyEffects;
            float[] matcherResults;
            object result = null;

            var expString = model.Model["m"]["m"].Value;
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
                for (var i = 0; i < policyLen; i++)
                {
                    var pvals = model.Model["p"]["p"].Policy[i];
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
                        var policyEft = parameters.FirstOrDefault(x => x.Name == "p_eft")?.Value as string;
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

                    if (effect.Equals("priority(p_eft) || deny"))
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
            result = eft.MergeEffects(effect, policyEffects, matcherResults);
            return (bool)result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rvals"></param>
        private Lambda GetAndInitializeExpression(object[] rvals)
        {
            var expString = model.Model["m"]["m"].Value;
            var parameters = GetParameters(rvals);
            var interpreter = GetAndInitializeInterpreter();
            var parsedExpression = interpreter.Parse(expString, parameters);
            return parsedExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        private Interpreter GetAndInitializeInterpreter()
        {
            var functions = new Dictionary<string, AbstractFunction>();
            foreach (var entry in fm.FunctionDict)
            {
                var key = entry.Key;
                var function = entry.Value;
                functions.Add(key, function);
            }

            if (model.Model.ContainsKey("g"))
            {
                foreach (var entry in model.Model["g"])
                {
                    var key = entry.Key;
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
            var rTokens = model.Model["r"]["r"]?.Tokens;
            var rTokensLen = rTokens?.Count();
            var parameters = new Dictionary<string, object>();
            for (var i = 0; i < rTokensLen; i++)
            {
                var token = rTokens[i];
                parameters.Add(token, rvals[i]);
            }
            for (int i = 0, length = model.Model["p"]["p"].Tokens.Length; i < length; i++)
            {
                var token = model.Model["p"]["p"].Tokens[i];
                if (pvals == null)
                {
                    parameters.Add(token, "");
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
