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
        public List<string> GetAllSubjects()
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
        public List<string> GetAllNamedSubjects(string ptype)
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
        public List<string> GetAllObjects()
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
        public List<string> GetAllNamedObjects(string ptype)
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
        public List<string> GetAllActions()
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
        public List<string> GetAllNamedActions(string ptype)
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
        public List<string> GetAllRoles()
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
        public List<string> GetAllNamedRoles(string ptype)
        {
            return model.GetValuesForFieldInPolicy("g", ptype, 1);
        }

        /// <summary>
        /// gets all the authorization rules in the policy.
        /// </summary>
        /// <returns> all the "p" policy rules.</returns>
        public List<List<string>> GetPolicy()
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
        public List<List<string>> GetFilteredPolicy(int fieldIndex, params string[] fieldValues)
        {
            return GetFilteredNamedPolicy("p", fieldIndex, fieldValues);
        }

        /// <summary>
        /// gets all the authorization rules in the named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>the "p" policy rules of the specified ptype.</returns>
        public List<List<string>> GetNamedPolicy(string ptype)
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
        public List<List<string>> GetFilteredNamedPolicy(string ptype, int fieldIndex, params string[] fieldValues)
        {
            return model.GetFilteredPolicy("p", ptype, fieldIndex, fieldValues);
        }

        /// <summary>
        /// gets all the role inheritance rules in the policy.
        /// </summary>
        /// <returns>all the "g" policy rules.</returns>
        public List<List<string>> GetGroupingPolicy()
        {
            return GetNamedGroupingPolicy("g");
        }

        /// <summary>
        /// gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex"> the policy rule's start index to be matched.</param>
        /// <param name="fieldValues">the field values to be matched, value "" means not to match this field.</param>
        /// <returns>the filtered "g" policy rules.</returns>
        public List<List<string>> GetFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues)
        {
            return GetFilteredNamedGroupingPolicy("g", fieldIndex, fieldValues);
        }

        /// <summary>
        /// gets all the role inheritance rules in the policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>the "g" policy rules of the specified ptype.</returns>
        public List<List<string>> GetNamedGroupingPolicy(string ptype)
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
        public List<List<string>> GetFilteredNamedGroupingPolicy(string ptype, int fieldIndex, params string[] fieldValues)
        {
            return model.GetFilteredPolicy("g", ptype, fieldIndex, fieldValues);
        }

        /// <summary>
        /// determines whether an authorization rule exists.
        /// </summary>
        /// <param name="paramList">the "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public bool HasPolicy(List<string> paramList)
        {
            return HasNamedPolicy("p", paramList);
        }

        /// <summary>
        /// determines whether an authorization rule exists.
        /// </summary>
        /// <param name="parmaters">the "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public bool HasPolicy(params string[] parmaters)
        {
            return HasPolicy(parmaters.ToList());
        }

        /// <summary>
        ///  determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="paramList">the "p" policy rule.</param>
        /// <returns>whether the rule exists.</returns>
        public bool HasNamedPolicy(string ptype, List<string> paramList)
        {
            return model.HasPolicy("p", ptype, paramList);
        }

        /// <summary>
        /// determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>whether the rule exists.</returns>
        public bool HasNamedPolicy(string ptype, params string[] parmaters)
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
        public bool AddPolicy(List<string> parmaters)
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
        public bool AddPolicy(params string[] parmaters)
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
        public bool AddNamedPolicy(string ptype, List<string> parmaters)
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
        public bool AddNamedPolicy(string ptype, params string[] parmaters)
        {
            return AddNamedPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parmaters">he "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemovePolicy(List<string> parmaters)
        {
            return RemoveNamedPolicy("p", parmaters);
        }


        /// <summary>
        /// removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parmaters">he "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemovePolicy(params string[] parmaters)
        {
            return RemovePolicy(parmaters.ToList());
        }

        /// <summary>
        /// removes an authorization rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">the policy rule's start index to be matched.</param>
        /// <param name="fieldValues"> the field values to be matched, value "" means not to match this field.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemoveFilteredPolicy(int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredNamedPolicy("p", fieldIndex, fieldValues);
        }

        /// <summary>
        /// removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemoveNamedPolicy(string ptype, List<string> parmaters)
        {
            return RemovePolicy("p", ptype, parmaters);
        }

        /// <summary>
        /// removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">the policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parmaters">the "p" policy rule.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemoveNamedPolicy(string ptype, params string[] parmaters)
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
        public bool RemoveFilteredNamedPolicy(string ptype, int fieldIndex, params string[] fieldValues)
        {
            return RemoveFilteredPolicy("p", ptype, fieldIndex, fieldValues);
        }

        /// <summary>
        /// determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="parmaters"> the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public bool HasGroupingPolicy(List<string> parmaters)
        {
            return HasNamedGroupingPolicy("g", parmaters);
        }

        /// <summary>
        /// determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="parmaters"> the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>whether the rule exists.</returns>
        public bool HasGroupingPolicy(params string[] parmaters)
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
        public bool HasNamedGroupingPolicy(string ptype, List<string> parmaters)
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
        public bool HasNamedGroupingPolicy(string ptype, params string[] parmaters)
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
        public bool AddGroupingPolicy(List<string> parmaters)
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
        public bool AddGroupingPolicy(params string[] parmaters)
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
        public bool AddNamedGroupingPolicy(string ptype, List<string> parmaters)
        {
            var ruleAdded = AddPolicy("g", ptype, parmaters);

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
        public bool AddNamedGroupingPolicy(string ptype, params string[] parmaters)
        {
            return AddNamedGroupingPolicy(ptype, parmaters.ToList());
        }

        /// <summary>
        /// removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parmaters">the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemoveGroupingPolicy(List<string> parmaters)
        {
            return RemoveNamedGroupingPolicy("g", parmaters);
        }

        /// <summary>
        /// removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parmaters">the "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>succeeds or not.</returns>
        public bool RemoveGroupingPolicy(params string[] parmaters)
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
        public bool RemoveFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues)
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
        public bool RemoveNamedGroupingPolicy(string ptype, List<string> parmaters)
        {
            var ruleRemoved = RemovePolicy("g", ptype, parmaters);

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
        public bool RemoveNamedGroupingPolicy(string ptype, params string[] parmaters)
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
        public bool RemoveFilteredNamedGroupingPolicy(string ptype, int fieldIndex, params string[] fieldValues)
        {
            var ruleRemoved = RemoveFilteredPolicy("g", ptype, fieldIndex, fieldValues);

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
        public void AddFunction(string name, AbstractFunction function)
        {
            fm.AddFunction(name, function);
        }
    }


}
