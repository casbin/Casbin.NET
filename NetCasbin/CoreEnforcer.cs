using DynamicExpresso;
using NetCasbin.Effect;
using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Rbac;
using NetCasbin.Util;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Model.Model m = new Model.Model();
            return m;
        }

        /// <summary>
        ///  creates a model.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Model.Model NewModel(String text)
        {
            Model.Model m = new Model.Model();
            m.LoadModelFromText(text);
            return m;
        }

        /// <summary>
        ///  creates a model.
        /// </summary>
        /// <param name="modelPath">the path of the model file.</param>
        /// <param name="unused">unused parameter, just for differentiating with  NewModel(String text).</param>
        /// <returns>the model.</returns>
        public static Model.Model NewModel(String modelPath, String unused)
        {
            Model.Model m = new Model.Model();
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
            model.LoadModel(this.modelPath);
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

        public void SetWatcher(IWatcher watcher)
        {
            this.watcher = watcher;
            //watcher.setUpdateCallback(this::loadPolicy);
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
            model.ClearPolicy();
            adapter.LoadPolicy(model);
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
            this.model.ClearPolicy();

            if (this.IsFiltered())
            {
                (this.adapter as IFilteredAdapter).LoadFilteredPolicy(this.model, filter);
            }
            else
            {
                throw new Exception("filtered policies are not supported by this adapter");
            }

            if (this.autoBuildRoleLinks)
            {
                this.BuildRoleLinks();
            }
            return true;
        }

        /// <summary>
        ///  returns true if the loaded policy has been filtered.
        /// </summary>
        /// <returns>if the loaded policy has been filtered.</returns>
        public Boolean IsFiltered()
        {
            if ((this.adapter is IFilteredAdapter))
            {
                return (this.adapter as IFilteredAdapter).IsFiltered;
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
            if (watcher != null)
            {
                watcher.Update();
            }
        }

        /// <summary>
        /// enableEnforce changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableEnforce(Boolean enable)
        {
            this._enabled = enable;
        }

        /// <summary>
        ///  enableAutoSave controls whether to save a policy rule automatically to the
        ///   adapter when it is added or removed.
        /// </summary>
        /// <param name="autoSave"></param>
        public void EnableAutoSave(Boolean autoSave)
        {
            this.autoSave = autoSave;
        }

        /// <summary>
        ///  controls whether to save a policy rule automatically
        ///   to the adapter when it is added or removed.
        /// </summary>
        /// <param name="autoBuildRoleLinks">whether to automatically build the role links.</param>
        public void EnableAutoBuildRoleLinks(Boolean autoBuildRoleLinks)
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
        public Boolean Enforce(params Object[] rvals)
        {
            if (!_enabled)
            {
                return true;
            }

            Dictionary<String, AbstractFunction> functions = new Dictionary<string, AbstractFunction>();
            foreach (var entry in fm.FunctionDict)
            {
                String key = entry.Key;
                var function = entry.Value;

                functions.Add(key, function);
            }
            if (model.Model.ContainsKey("g"))
            {
                foreach (var entry in model.Model["g"])
                {
                    String key = entry.Key;
                    Assertion ast = entry.Value;
                    IRoleManager rm = ast.RM;
                    functions.Add(key, BuiltInFunctions.GenerateGFunction(key, rm));
                }
            }

            String expString = model.Model["m"]["m"].Value;
            var interpreter = new Interpreter();
            foreach (var func in functions)
            {
                interpreter.SetFunction(func.Key, func.Value);
            }

            Effect.Effect[] policyEffects;
            float[] matcherResults;
            int policyLen;
            object result = null;
            if ((policyLen = model.Model["p"]["p"].Policy.Count) != 0)
            {
                policyEffects = new Effect.Effect[policyLen];
                matcherResults = new float[policyLen];

                for (int i = 0; i < model.Model["p"]["p"].Policy.Count; i++)
                {
                    List<String> pvals = model.Model["p"]["p"].Policy[i];
                    Dictionary<String, Object> parameters = new Dictionary<string, object>();
                    for (int j = 0; j < model.Model["r"]["r"].Tokens.Length; j++)
                    {
                        String token = model.Model["r"]["r"].Tokens[j];
                        parameters.Add(token, rvals[j]);
                    }
                    for (int j = 0; j < model.Model["p"]["p"].Tokens.Length; j++)
                    {
                        String token = model.Model["p"]["p"].Tokens[j];
                        parameters.Add(token, pvals[j]);
                    }
                    foreach (var item in parameters)
                    {
                        interpreter.SetVariable(item.Key, item.Value);
                    }
                    result = interpreter.Eval(expString);
                    if (result is Boolean)
                    {
                        if (!((Boolean)result))
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
                    if (parameters.ContainsKey("p_eft"))
                    {
                        String eft = (String)parameters["p_eft"];
                        if (eft.Equals("allow"))
                        {
                            policyEffects[i] = Effect.Effect.Allow;
                        }
                        else if (eft.Equals("deny"))
                        {
                            policyEffects[i] = Effect.Effect.Deny;
                        }
                        else
                        {
                            policyEffects[i] = Effect.Effect.Indeterminate;
                        }
                    }
                    else
                    {
                        policyEffects[i] = Effect.Effect.Allow;
                    }

                    if (model.Model["e"]["e"].Value.Equals("priority(p_eft) || deny"))
                    {
                        break;
                    }
                }
            }
            else
            {
                policyEffects = new Effect.Effect[1];
                matcherResults = new float[1];

                Dictionary<String, Object> parameters = new Dictionary<string, Object>();
                for (int j = 0; j < model.Model["r"]["r"].Tokens.Length; j++)
                {
                    String token = model.Model["r"]["r"].Tokens[j];
                    parameters.Add(token, rvals[j]);
                }
                for (int j = 0; j < model.Model["p"]["p"].Tokens.Length; j++)
                {
                    String token = model.Model["p"]["p"].Tokens[j];
                    parameters.Add(token, "");
                }

                foreach (var item in parameters)
                {
                    interpreter.SetVariable(item.Key, item.Value);
                }

                result = interpreter.Eval(expString, parameters.Select(x => new Parameter(x.Key, x.Value)).ToArray());

                if ((Boolean)result)
                {
                    policyEffects[0] = Effect.Effect.Allow;
                }
                else
                {
                    policyEffects[0] = Effect.Effect.Indeterminate;
                }
            }
            result = eft.MergeEffects(model.Model["e"]["e"].Value, policyEffects, matcherResults);
            return (Boolean)result;
        }
    }
}
