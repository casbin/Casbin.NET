using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCasbin
{
    /// <summary>
    ///  ManagementEnforcer = InternalEnforcer + Management API.
    /// </summary>
    public class ManagementEnforcer : InternalEnforcer
    {
        /// <summary>
        /// gets the list of subjects that show up in the current policy.
        /// </summary>
        /// <returns>
        /// all the subjects in "p" policy rules. It actually collects the
        ///  0-index elements of "p" policy rules. So make sure your subject
        ///  is the 0-index element, like (sub, obj, act). Duplicates are removed.
        /// </returns>
        public List<String> GetAllSubjects()
        {
            return GetAllNamedSubjects("p");
        }

        public List<String> GetAllNamedSubjects(String ptype)
        {
            return model.GetValuesForFieldInPolicy("p", ptype, 0);
        }

        public List<String> getAllObjects()
        {
            return GetAllNamedObjects("p");
        }

        public List<String> GetAllNamedObjects(String ptype)
        {
            return model.GetValuesForFieldInPolicy("p", ptype, 1);
        }

        public List<String> GetAllActions()
        {
            return GetAllNamedActions("p");
        }

        public List<String> GetAllNamedActions(String ptype)
        {
            return model.GetValuesForFieldInPolicy("p", ptype, 2);
        }

        public List<String> GetAllRoles()
        {
            return GetAllNamedRoles("g");
        }
        public List<String> GetAllNamedRoles(String ptype)
        {
            return model.GetValuesForFieldInPolicy("g", ptype, 1);
        }

        public List<List<String>> GetPolicy()
        {
            return GetNamedPolicy("p");
        }

        public List<List<String>> GetFilteredPolicy(int fieldIndex, params string[] fieldValues)
        {
            return GetFilteredNamedPolicy("p", fieldIndex, fieldValues);
        }

        public List<List<String>> GetNamedPolicy(String ptype)
        {
            return model.GetPolicy("p", ptype);
        }

        public List<List<String>> GetFilteredNamedPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            return model.GetFilteredPolicy("p", ptype, fieldIndex, fieldValues);
        }
        public List<List<String>> getGroupingPolicy()
        {
            return GetNamedGroupingPolicy("g");
        }

        public List<List<String>> GetFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues)
        {
            return GetFilteredNamedGroupingPolicy("g", fieldIndex, fieldValues);
        }

        public List<List<String>> GetNamedGroupingPolicy(String ptype)
        {
            return model.GetPolicy("g", ptype);
        }

        public List<List<String>> GetFilteredNamedGroupingPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            return model.GetFilteredPolicy("g", ptype, fieldIndex, fieldValues);
        }

        public Boolean HasPolicy(List<String> paramList)
        {
            return HasNamedPolicy("p", paramList);
        }

        public Boolean HasPolicy(params string[] parmaters)
        {
            return HasPolicy(parmaters.ToList());
        }

        public Boolean HasNamedPolicy(String ptype, List<String> paramList)
        {
            return model.HasPolicy("p", ptype, paramList);
        }

        public Boolean HasNamedPolicy(String ptype, params string[] parmaters)
        {
            return HasNamedPolicy(ptype, parmaters.ToList());
        }

        public Boolean AddPolicy(List<String> parmaters)
        {
            return AddNamedPolicy("p", parmaters);
        }

        public Boolean AddPolicy(params string[] parmaters)
        {
            return AddPolicy(parmaters.ToList());
        }

        public Boolean AddNamedPolicy(String ptype, List<String> parmaters)
        {
            return AddPolicy("p", ptype, parmaters);
        }

        public Boolean AddNamedPolicy(String ptype, params string[] parmaters)
        {
            return AddNamedPolicy(ptype, parmaters.ToList());
        }

        public Boolean RemovePolicy(List<String> parmaters)
        {
            return RemoveNamedPolicy("p", parmaters);
        }

        public Boolean RemovePolicy(params string[] parmaters)
        {
            return RemovePolicy(parmaters.ToList());
        }

        public Boolean RemoveFilteredPolicy(int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredNamedPolicy("p", fieldIndex, fieldValues);
        }

        public Boolean RemoveNamedPolicy(String ptype, List<String> parmaters)
        {
            return RemovePolicy("p", ptype, parmaters);
        }

        public Boolean RemoveNamedPolicy(String ptype, params string[] parmaters)
        {
            return RemoveNamedPolicy(ptype, parmaters.ToList());
        }

        public Boolean RemoveFilteredNamedPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredPolicy("p", ptype, fieldIndex, fieldValues);
        }

        public Boolean HasGroupingPolicy(List<String> parmaters)
        {
            return HasNamedGroupingPolicy("g", parmaters);
        }

        public Boolean HasGroupingPolicy(params string[] parmaters)
        {
            return HasGroupingPolicy(parmaters.ToList());
        }

        public Boolean HasNamedGroupingPolicy(String ptype, List<String> parmaters)
        {
            return model.HasPolicy("g", ptype, parmaters);
        }

        public Boolean HasNamedGroupingPolicy(String ptype, params string[] parmaters)
        {
            return HasNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        public Boolean AddGroupingPolicy(List<String> parmaters)
        {
            return AddNamedGroupingPolicy("g", parmaters);
        }

        public Boolean AddGroupingPolicy(params string[] parmaters)
        {
            return AddGroupingPolicy(parmaters.ToList());
        }

        public Boolean AddNamedGroupingPolicy(String ptype, List<String> parmaters)
        {
            Boolean ruleAdded = AddPolicy("g", ptype, parmaters);

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
            return ruleAdded;
        }

        public Boolean AddNamedGroupingPolicy(String ptype, params string[] parmaters)
        {
            return AddNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        public Boolean RemoveGroupingPolicy(List<String> parmaters)
        {
            return RemoveNamedGroupingPolicy("g", parmaters);
        }

        public Boolean RemoveGroupingPolicy(params string[] parmaters)
        {
            return RemoveGroupingPolicy(parmaters.ToList());
        }

        public Boolean RemoveFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredNamedGroupingPolicy("g", fieldIndex, fieldValues);
        }

        public Boolean RemoveNamedGroupingPolicy(String ptype, List<String> parmaters)
        {
            Boolean ruleRemoved = RemovePolicy("g", ptype, parmaters);

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
            return ruleRemoved;
        }

        public Boolean RemoveNamedGroupingPolicy(String ptype, params string[] parmaters)
        {
            return RemoveNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        public Boolean RemoveFilteredNamedGroupingPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            Boolean ruleRemoved = RemoveFilteredPolicy("g", ptype, fieldIndex, fieldValues);

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
            return ruleRemoved;
        }

        /// <summary>
        /// adds a customized function.
        /// </summary>
        /// <param name="name">the name of the new function.</param>
        /// <param name="function">the function.</param>
        public void AddFunction(String name, AbstractFunction function)
        {
            fm.AddFunction(name, function);
        }
    }


}
