using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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

        /// <summary>
        /// GetAllNamedSubjects gets the list of subjects that show up in the currentnamed policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>all the subjects in policy rules of the ptype type. It actually
        ///         collects the 0-index elements of the policy rules.So make sure
        ///         your subject is the 0-index element, like (sub, obj, act).
        //          Duplicates are removed.</returns>
        public List<String> GetAllNamedSubjects(String ptype)
        {
            return model.GetValuesForFieldInPolicy("p", ptype, 0);
        }

        /// <summary>
        /// gets the list of objects that show up in the current policy.
        /// </summary>
        /// <returns>
        /// all the objects in "p" policy rules. It actually collects the
        /// 1-index elements of "p" policy rules.So make sure your object
        /// is the 1-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public List<String> GetAllObjects()
        {
            return GetAllNamedObjects("p");
        }

        /// <summary>
        /// gets the list of objects that show up in the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>all the objects in policy rules of the ptype type. It actually
        /// collects the 1-index elements of the policy rules.So make sure
        /// your object is the 1-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public List<String> GetAllNamedObjects(String ptype)
        {
            return model.GetValuesForFieldInPolicy("p", ptype, 1);
        }

        /// <summary>
        /// gets the list of actions that show up in the current policy.
        /// </summary>
        /// <returns>
        /// all the actions in "p" policy rules. It actually collects
        /// the 2-index elements of "p" policy rules.So make sure your action
        /// is the 2-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public List<String> GetAllActions()
        {
            return GetAllNamedActions("p");
        }

        /// <summary>
        ///  GetAllNamedActions gets the list of actions that show up in the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// all the actions in policy rules of the ptype type. It actually
        /// collects the 2-index elements of the policy rules.So make sure
        /// your action is the 2-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public List<String> GetAllNamedActions(String ptype)
        {
            return model.GetValuesForFieldInPolicy("p", ptype, 2);
        }

        /// <summary>
        ///  gets the list of roles that show up in the current policy.
        /// </summary>
        /// <returns>
        /// all the roles in "g" policy rules. It actually collects
        /// the 1-index elements of "g" policy rules. So make sure your
        /// role is the 1-index element, like (sub, role).
        /// Duplicates are removed.</returns>
        public List<String> GetAllRoles()
        {
            return GetAllNamedRoles("g");
        }

        /// <summary>
        /// gets the list of roles that show up in the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>
        /// all the subjects in policy rules of the ptype type. It actually
        /// collects the 0-index elements of the policy rules.So make
        /// sure your subject is the 0-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public List<String> GetAllNamedRoles(String ptype)
        {
            return model.GetValuesForFieldInPolicy("g", ptype, 1);
        }

        /// <summary>
        /// gets all the authorization rules in the policy.
        /// </summary>
        /// <returns> all the "p" policy rules.</returns>
        public List<List<String>> GetPolicy()
        {
            return GetNamedPolicy("p");
        }

        /// <summary>
        /// getFilteredPolicy gets all the authorization rules in the policy, field
        /// filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to  match this field.</param>
        /// <returns>the filtered "p" policy rules.</returns>
        public List<List<String>> GetFilteredPolicy(int fieldIndex, params string[] fieldValues)
        {
            return GetFilteredNamedPolicy("p", fieldIndex, fieldValues);
        }

        /// <summary>
        /// gets all the authorization rules in the named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>the "p" policy rules of the specified ptype.</returns>
        public List<List<String>> GetNamedPolicy(String ptype)
        {
            return model.GetPolicy("p", ptype);
        }

        /// <summary>
        /// gets all the authorization rules in the named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype"> the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to  match this field.</param>
        /// <returns>the filtered "p" policy rules of the specified ptype.</returns>
        public List<List<String>> GetFilteredNamedPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            return model.GetFilteredPolicy("p", ptype, fieldIndex, fieldValues);
        }

        /// <summary>
        /// gets all the role inheritance rules in the policy.
        /// </summary>
        /// <returns>all the "g" policy rules.</returns>
        public List<List<String>> GetGroupingPolicy()
        {
            return GetNamedGroupingPolicy("g");
        }

        /// <summary>
        /// gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex"> the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>the filtered "g" policy rules.</returns>
        public List<List<String>> GetFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues)
        {
            return GetFilteredNamedGroupingPolicy("g", fieldIndex, fieldValues);
        }

        /// <summary>
        /// gets all the role inheritance rules in the policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>the "g" policy rules of the specified ptype.</returns>
        public List<List<String>> GetNamedGroupingPolicy(String ptype)
        {
            return model.GetPolicy("g", ptype);
        }

        /// <summary>
        ///  gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>the filtered "g" policy rules of the specified ptype.</returns>
        public List<List<String>> GetFilteredNamedGroupingPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            return model.GetFilteredPolicy("g", ptype, fieldIndex, fieldValues);
        }

        /// <summary>
        /// determines whether an authorization rule exists.
        /// </summary>
        /// <param name="paramList">the "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasPolicy(List<String> paramList)
        {
            return HasNamedPolicy("p", paramList);
        }

        /// <summary>
        /// determines whether an authorization rule exists.
        /// </summary>
        /// <param name="parmaters">the "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasPolicy(params string[] parmaters)
        {
            return HasPolicy(parmaters.ToList());
        }

        /// <summary>
        ///  determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="paramList">the "p" policy rule.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasNamedPolicy(String ptype, List<String> paramList)
        {
            return model.HasPolicy("p", ptype, paramList);
        }

        /// <summary>
        /// determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasNamedPolicy(String ptype, params string[] parmaters)
        {
            return HasNamedPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// addPolicy adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parmaters">the "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddPolicy(List<String> parmaters)
        {
            return AddNamedPolicy("p", parmaters);
        }

        /// <summary>
        /// addPolicy adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parmaters">the "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddPolicy(params string[] parmaters)
        {
            return AddPolicy(parmaters.ToList());
        }

        /// <summary>
        /// AddNamedPolicy adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be
        /// added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddNamedPolicy(String ptype, List<String> parmaters)
        {
            return AddPolicy("p", ptype, parmaters);
        }

        /// <summary>
        /// AddNamedPolicy adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be
        /// added.Otherwise the function returns true by adding the new rule.
        /// </summary>>
        /// <param name="ptype"> the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddNamedPolicy(String ptype, params string[] parmaters)
        {
            return AddNamedPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parmaters">he "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemovePolicy(List<String> parmaters)
        {
            return RemoveNamedPolicy("p", parmaters);
        }


        /// <summary>
        /// removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parmaters">he "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemovePolicy(params string[] parmaters)
        {
            return RemovePolicy(parmaters.ToList());
        }

        /// <summary>
        /// removes an authorization rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues"> the field values to be matched, value "" means not to match this field.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveFilteredPolicy(int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredNamedPolicy("p", fieldIndex, fieldValues);
        }

        /// <summary>
        /// removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveNamedPolicy(String ptype, List<String> parmaters)
        {
            return RemovePolicy("p", ptype, parmaters);
        }

        /// <summary>
        /// removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveNamedPolicy(String ptype, params string[] parmaters)
        {
            return RemoveNamedPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        ///  removes an authorization rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to  match this field.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveFilteredNamedPolicy(String ptype, int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredPolicy("p", ptype, fieldIndex, fieldValues);
        }

        /// <summary>
        /// determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="parmaters"> the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasGroupingPolicy(List<String> parmaters)
        {
            return HasNamedGroupingPolicy("g", parmaters);
        }

        /// <summary>
        /// determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="parmaters"> the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasGroupingPolicy(params string[] parmaters)
        {
            return HasGroupingPolicy(parmaters.ToList());
        }

        /// <summary>
        /// hasNamedGroupingPolicy determines whether a named role inheritance rule
        /// exists.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parmaters"> the "g" policy rule.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasNamedGroupingPolicy(String ptype, List<String> parmaters)
        {
            return model.HasPolicy("g", ptype, parmaters);
        }

        /// <summary>
        /// hasNamedGroupingPolicy determines whether a named role inheritance rule
        /// exists.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parmaters"> the "g" policy rule.</param>
        /// <returns>whether the rule exists.</returns>
        public Boolean HasNamedGroupingPolicy(String ptype, params string[] parmaters)
        {
            return HasNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parmaters">the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddGroupingPolicy(List<String> parmaters)
        {
            return AddNamedGroupingPolicy("g", parmaters);
        }

        /// <summary>
        /// adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parmaters">the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddGroupingPolicy(params string[] parmaters)
        {
            return AddGroupingPolicy(parmaters.ToList());
        }

        /// <summary>
        /// adds a named role inheritance rule to the current 
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parmaters">the "g" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddNamedGroupingPolicy(String ptype, List<String> parmaters)
        {
            Boolean ruleAdded = AddPolicy("g", ptype, parmaters);

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
            return ruleAdded;
        }

        /// <summary>
        /// adds a named role inheritance rule to the current 
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parmaters">the "g" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean AddNamedGroupingPolicy(String ptype, params string[] parmaters)
        {
            return AddNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parmaters">the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveGroupingPolicy(List<String> parmaters)
        {
            return RemoveNamedGroupingPolicy("g", parmaters);
        }

        /// <summary>
        /// removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parmaters">the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveGroupingPolicy(params string[] parmaters)
        {
            return RemoveGroupingPolicy(parmaters.ToList());
        }

        /// <summary>
        /// removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredNamedGroupingPolicy("g", fieldIndex, fieldValues);
        }

        /// <summary>
        /// removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveNamedGroupingPolicy(String ptype, List<String> parmaters)
        {
            Boolean ruleRemoved = RemovePolicy("g", ptype, parmaters);

            if (autoBuildRoleLinks)
            {
                BuildRoleLinks();
            }
            return ruleRemoved;
        }

        /// <summary>
        /// removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>succeeds or not.</returns>
        public Boolean RemoveNamedGroupingPolicy(String ptype, params string[] parmaters)
        {
            return RemoveNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// removes a role inheritance rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype"> the policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>succeeds or not.</returns>
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
