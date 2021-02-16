using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCasbin.Abstractions
{
    /// <summary>
    /// IManagementEnforcer is the API interface of ManagementEnforcer
    /// </summary>
    [Obsolete("The interface will be removed at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/56 to know more information.")]
    public interface IManagementEnforcer : ICoreEnforcer
    {
        /// <summary>
        /// Gets the list of subjects that show up in the current policy.
        /// </summary>
        /// <returns>
        /// All the subjects in "p" policy rules. It actually collects the
        /// 0-index elements of "p" policy rules. So make sure your subject
        /// is the 0-index element, like (sub, obj, act). Duplicates are removed.
        /// </returns>
        List<string> GetAllSubjects();

        /// <summary>
        /// GetAllNamedSubjects gets the list of subjects that show up in the currentnamed policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// All the subjects in policy rules of the ptype type. It actually
        /// collects the 0-index elements of the policy rules.So make sure
        /// your subject is the 0-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        List<string> GetAllNamedSubjects(string ptype);

        /// <summary>
        /// Gets the list of objects that show up in the current policy.
        /// </summary>
        /// <returns>
        /// All the objects in "p" policy rules. It actually collects the
        /// 1-index elements of "p" policy rules.So make sure your object
        /// is the 1-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        List<string> GetAllObjects();

        /// <summary>
        /// Gets the list of objects that show up in the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// All the objects in policy rules of the ptype type. It actually
        /// collects the 1-index elements of the policy rules.So make sure
        /// your object is the 1-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        List<string> GetAllNamedObjects(string ptype);

        /// <summary>
        /// Gets the list of actions that show up in the current policy.
        /// </summary>
        /// <returns>
        /// All the actions in "p" policy rules. It actually collects
        /// the 2-index elements of "p" policy rules.So make sure your action
        /// is the 2-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        List<string> GetAllActions();

        /// <summary>
        /// Gets the list of actions that show up in the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// All the actions in policy rules of the ptype type. It actually
        /// collects the 2-index elements of the policy rules.So make sure
        /// your action is the 2-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        List<string> GetAllNamedActions(string ptype);

        /// <summary>
        /// Gets the list of roles that show up in the current policy.
        /// </summary>
        /// <returns>
        /// All the roles in "g" policy rules. It actually collects
        /// the 1-index elements of "g" policy rules. So make sure your
        /// role is the 1-index element, like (sub, role).
        /// Duplicates are removed.</returns>
        List<string> GetAllRoles();

        /// <summary>
        /// Gets the list of roles that show up in the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>
        /// All the subjects in policy rules of the ptype type. It actually
        /// collects the 0-index elements of the policy rules.So make
        /// Sure your subject is the 0-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        List<string> GetAllNamedRoles(string ptype);

        /// <summary>
        /// Gets all the authorization rules in the policy.
        /// </summary>
        /// <returns> all the "p" policy rules.</returns>
        List<List<string>> GetPolicy();

        /// <summary>
        /// Gets all the authorization rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>The filtered "p" policy rules.</returns>
        List<List<string>> GetFilteredPolicy(int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Gets all the authorization rules in the named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>The "p" policy rules of the specified ptype.</returns>
        List<List<string>> GetNamedPolicy(string ptype);

        /// <summary>
        /// Gets all the authorization rules in the named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>The filtered "p" policy rules of the specified ptype.</returns>
        List<List<string>> GetFilteredNamedPolicy(string ptype, int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Gets all the role inheritance rules in the policy.
        /// </summary>
        /// <returns>all the "g" policy rules.</returns>
        List<List<string>> GetGroupingPolicy();

        /// <summary>
        /// Gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>The filtered "g" policy rules.</returns>
        List<List<string>> GetFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Gets all the role inheritance rules in the policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>The "g" policy rules of the specified ptype.</returns>
        List<List<string>> GetNamedGroupingPolicy(string ptype);

        /// <summary>
        /// Gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>The filtered "g" policy rules of the specified ptype.</returns>
        List<List<string>> GetFilteredNamedGroupingPolicy(string ptype, int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Determines whether an authorization rule exists.
        /// </summary>
        /// <param name="paramList">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasPolicy(List<string> paramList);

        /// <summary>
        /// Determines whether an authorization rule exists.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasPolicy(params string[] parameters);

        /// <summary>
        /// Determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="paramList">The "p" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasNamedPolicy(string ptype, List<string> paramList);

        /// <summary>
        /// Determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasNamedPolicy(string ptype, params string[] parameters);

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddPolicy(params string[] parameters);

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddPolicyAsync(params string[] parameters);

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddPolicy(List<string> parameters);

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddPolicyAsync(List<string> parameters);

        /// <summary>
        /// Adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddNamedPolicy(string ptype, params string[] parameters);

        /// <summary>
        /// Adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddNamedPolicyAsync(string ptype, params string[] parameters);

        /// <summary>
        /// Adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddNamedPolicy(string ptype, List<string> parameters);

        /// <summary>
        /// Adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddNamedPolicyAsync(string ptype, List<string> parameters);

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemovePolicy(params string[] parameters);

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemovePolicyAsync(params string[] parameters);

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemovePolicy(List<string> parameters);

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "p" policy rule, ptype "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemovePolicyAsync(List<string> parameters);

        /// <summary>
        /// Removes an authorization rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveFilteredPolicy(int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Removes an authorization rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveFilteredPolicyAsync(int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveNamedPolicy(string ptype, params string[] parameters);

        /// <summary>
        /// Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveNamedPolicyAsync(string ptype, params string[] parameters);

        /// <summary>
        /// Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveNamedPolicy(string ptype, List<string> parameters);

        /// <summary>
        /// Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="parameters">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveNamedPolicyAsync(string ptype, List<string> parameters);

        /// <summary>
        /// Removes an authorization rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveFilteredNamedPolicy(string ptype, int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Removes an authorization rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveFilteredNamedPolicyAsync(string ptype, int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasGroupingPolicy(List<string> parameters);

        /// <summary>
        /// Determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasGroupingPolicy(params string[] parameters);

        /// <summary>
        /// Determines whether a named role inheritance rule
        /// exists.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasNamedGroupingPolicy(string ptype, List<string> parameters);

        /// <summary>
        /// Determines whether a named role inheritance rule
        /// exists.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        bool HasNamedGroupingPolicy(string ptype, params string[] parameters);

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddGroupingPolicy(params string[] parameters);

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddGroupingPolicyAsync(params string[] parameters);

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddGroupingPolicy(List<string> parameters);

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddGroupingPolicyAsync(List<string> parameters);

        /// <summary>
        /// Adds a named role inheritance rule to the current 
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddNamedGroupingPolicy(string ptype, List<string> parameters);

        /// <summary>
        /// Adds a named role inheritance rule to the current 
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> AddNamedGroupingPolicyAsync(string ptype, List<string> parameters);

        /// <summary>
        /// Adds a named role inheritance rule to the current 
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        bool AddNamedGroupingPolicy(string ptype, params string[] parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveGroupingPolicy(params string[] parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveGroupingPolicyAsync(params string[] parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveGroupingPolicy(List<string> parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveGroupingPolicyAsync(List<string> parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveNamedGroupingPolicy(string ptype, params string[] parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveNamedGroupingPolicyAsync(string ptype, params string[] parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveNamedGroupingPolicy(string ptype, List<string> parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="parameters">The "g" policy rule, ptype "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveNamedGroupingPolicyAsync(string ptype, List<string> parameters);

        /// <summary>
        /// Removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveFilteredGroupingPolicy(int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Removes a role inheritance rule from the current 
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveFilteredGroupingPolicyAsync(int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Removes a role inheritance rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        bool RemoveFilteredNamedGroupingPolicy(string ptype, int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Removes a role inheritance rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="ptype">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        Task<bool> RemoveFilteredNamedGroupingPolicyAsync(string ptype, int fieldIndex, params string[] fieldValues);

        /// <summary>
        /// Adds a customized function.
        /// </summary>
        /// <param name="name">The name of the new function.</param>
        /// <param name="function">The function.</param>
        void AddFunction(string name, Delegate function);
    }

}
