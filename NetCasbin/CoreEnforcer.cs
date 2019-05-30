using DynamicExpresso;
using NetCasbin.Persist;
using NetCasbin.Rabc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NetCasbin
{
    public class CoreEnforcer
    {
        private IEffector eft;
        private Boolean _enabled;

        protected String modelPath;
        protected Model model;
        protected FunctionMap fm;

        protected IAdapter adapter;
        protected IWatcher watcher;
        protected IRoleManager rm;
        protected Boolean autoSave;
        protected Boolean autoBuildRoleLinks;

        protected void Initialize()
        {
            rm = new DefaultRoleManager(10);
            eft = new DefaultEffector();
            watcher = null;

            _enabled = true;
            autoSave = true;
            autoBuildRoleLinks = true;
        }

        public static Model NewModel()
        {
            Model m = new Model();
            return m;
        }

        public static Model NewModel(String text)
        {
            Model m = new Model();
            m.LoadModelFromText(text);
            return m;
        }

        public static Model NewModel(String modelPath, String unused)
        {
            Model m = new Model();
            if (!string.IsNullOrEmpty(modelPath))
            {
                m.LoadModel(modelPath);
            }

            return m;
        }

        public void LoadModel()
        {
            model = NewModel();
            model.LoadModel(this.modelPath);
            fm = FunctionMap.LoadFunctionMap();
        }

        public Model GetModel() => model;

        public void SetModel(Model model)
        {
            this.model = model;
            fm = FunctionMap.LoadFunctionMap();
        }

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

        public void SetRoleManager(IRoleManager rm)
        {
            this.rm = rm;
        }

        public void SetEffector(IEffector eft)
        {
            this.eft = eft;
        }

        public void ClearPolicy()
        {
            model.ClearPolicy();
        }

        public void LoadPolicy()
        {
            model.ClearPolicy();
            adapter.LoadPolicy(model);
            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
        }

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

        public Boolean IsFiltered()
        {
            if ((this.adapter is IFilteredAdapter))
            {
                return (this.adapter as IFilteredAdapter).IsFiltered;
            }
            return false;
        }

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

        public void EnableEnforce(Boolean enable)
        {
            this._enabled = enable;
        }

        public void EnableAutoSave(Boolean autoSave)
        {
            this.autoSave = autoSave;
        }

        public void EnableAutoBuildRoleLinks(Boolean autoBuildRoleLinks)
        {
            this.autoBuildRoleLinks = autoBuildRoleLinks;
        }

        public void BuildRoleLinks()
        {
            rm.Clear();
            model.BuildRoleLinks(rm);
        }

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

            Effect[] policyEffects;
            float[] matcherResults;
            int policyLen;
            object result = null;
            if ((policyLen = model.Model["p"]["p"].Policy.Count) != 0)
            {
                policyEffects = new Effect[policyLen];
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
                            policyEffects[i] = Effect.Indeterminate;
                            continue;
                        }
                    }
                    else if (result is float)
                    {
                        if ((float)result == 0)
                        {
                            policyEffects[i] = Effect.Indeterminate;
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
                            policyEffects[i] = Effect.Allow;
                        }
                        else if (eft.Equals("deny"))
                        {
                            policyEffects[i] = Effect.Deny;
                        }
                        else
                        {
                            policyEffects[i] = Effect.Indeterminate;
                        }
                    }
                    else
                    {
                        policyEffects[i] = Effect.Allow;
                    }

                    if (model.Model["e"]["e"].Value.Equals("priority(p_eft) || deny"))
                    {
                        break;
                    }
                }
            }
            else
            {
                policyEffects = new Effect[1];
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
                    policyEffects[0] = Effect.Allow;
                }
                else
                {
                    policyEffects[0] = Effect.Indeterminate;
                }
            }
            result = eft.MergeEffects(model.Model["e"]["e"].Value, policyEffects, matcherResults);
            return (Boolean)result;
        }
    }
}
