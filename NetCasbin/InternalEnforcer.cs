using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin
{
    public class InternalEnforcer : CoreEnforcer
    {
        protected Boolean AddPolicy(String sec, String ptype, List<String> rule)
        {
            Boolean ruleAdded = model.AddPolicy(sec, ptype, rule);
            if (!ruleAdded)
            {
                return false;
            }

            if (adapter != null && autoSave)
            {
                try
                {
                    adapter.AddPolicy(sec, ptype, rule);
                }
                catch (NotImplementedException e)
                {

                }
                catch (Exception e)
                {
                    throw e;
                }

                if (watcher != null)
                {
                    // error intentionally ignored
                    watcher.Update();
                }
            }
            return true;
        }

        protected Boolean RemovePolicy(String sec, String ptype, List<String> rule)
        {
            Boolean ruleRemoved = model.RemovePolicy(sec, ptype, rule);
            if (!ruleRemoved)
            {
                return false;
            }

            if (adapter != null && autoSave)
            {
                try
                {
                    adapter.RemovePolicy(sec, ptype, rule);
                }
                catch (NotImplementedException e)
                { }
                catch (Exception e)
                {
                    throw e;
                }

                if (watcher != null)
                {
                    // error intentionally ignored
                    watcher.Update();
                }
            }
            return true;
        }

        protected Boolean RemoveFilteredPolicy(String sec, String ptype, int fieldIndex, params string[] fieldValues)
        {
            Boolean ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
            if (!ruleRemoved)
            {
                return false;
            }

            if (adapter != null && autoSave)
            {
                try
                {
                    adapter.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
                }
                catch (NotImplementedException e)
                { }
                catch (Exception e)
                {
                    throw e;
                }

                if (watcher != null)
                {
                    // error intentionally ignored
                    watcher.Update();
                }
            }
            return true;
        }
    }
}
