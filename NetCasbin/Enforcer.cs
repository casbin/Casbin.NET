using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetCasbin.Abstractions;
using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;

namespace NetCasbin
{
    /// <summary>
    /// Enforcer = ManagementEnforcer + RBAC API.
    /// </summary>
    public class Enforcer : ManagementEnforcer, IEnforcer
    {
        public Enforcer() : this(string.Empty, string.Empty)
        {
        }

        public Enforcer(string modelPath, string policyPath)
            : this(modelPath, new DefaultFileAdapter(policyPath))
        {
        }

        public Enforcer(string modelPath, IAdapter adapter)
            : this(NewModel(modelPath, string.Empty), adapter)
        {
            this.modelPath = modelPath;
        }

        public Enforcer(Model.Model model, IAdapter adapter)
        {
            this.adapter = adapter;
            watcher = null;
            SetModel(model);
            Initialize();
            LoadPolicy();
        }

        public Enforcer(Model.Model m)
            : this(m, null)
        {
        }

        public Enforcer(string modelPath)
            : this(modelPath, string.Empty)
        {
        }

        [Obsolete("The method will be removed at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/116 to know more information.")]
        public Enforcer(string modelPath, string policyFile, bool enableLog)
            : this(modelPath, new DefaultFileAdapter(policyFile))
        {

        }

        #region Get roles or users

        /// <summary>
        /// Gets the roles that a user has.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetRolesForUser(string name,  string domain = null)
        {
            return domain is null
                ? model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles(name)
                : model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles(name, domain);
        }

        /// <summary>
        /// Gets the users that has a role.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetUsersForRole(string name, string domain = null)
        {
            return domain is null
                ? model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers(name)
                : model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers(name, domain);
        }

        /// <summary>
        /// Gets the users that has roles.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<string> GetUsersForRoles(string[] names)
        {
            var userIds = new List<string>();
            foreach (string name in names)
            {
                userIds.AddRange(model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers(name));
            }
            return userIds;
        }

        #endregion

        #region Has roles or users

        /// <summary>
        /// Determines whether a user has a role.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool HasRoleForUser(string name, string role, string domain = null)
        {
            var roles = GetRolesForUser(name, domain);
            return roles.Any(roleEnum => roleEnum.Equals(role));
        }

        #endregion

        #region Add roles or users

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public bool AddRoleForUser(string user, string role, string domain = null)
        {
            return domain is null
                ? AddGroupingPolicy(user, role)
                : AddGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public Task<bool> AddRoleForUserAsync(string user, string role, string domain = null)
        {
            return domain is null
                ? AddGroupingPolicyAsync(user, role)
                : AddGroupingPolicyAsync(user, role, domain);
        }

        /// <summary>
        /// AddRolesForUser adds roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public bool AddRolesForUser(string user, IEnumerable<string> role, string domain = null)
        {
            return domain is null
                ? AddGroupingPolicies(role.Select(r => new List<string>{user, r}))
                : AddGroupingPolicies(role.Select(r => new List<string>{user, r, domain}));
        }

        /// <summary>
        /// AddRolesForUser adds roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public Task<bool> AddRolesForUserAsync(string user, IEnumerable<string> role, string domain = null)
        {
            return domain is null
                ? AddGroupingPoliciesAsync(role.Select(r => new List<string> {user, r}))
                : AddGroupingPoliciesAsync(role.Select(r => new List<string> {user, r, domain}));
        }

        #endregion

        #region Delete roles or users

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have the role (aka not affected).</returns>
        public bool DeleteRoleForUser(string user, string role, string domain = null)
        {
            return domain is null
                ? RemoveGroupingPolicy(user, role)
                : RemoveGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// Deletes a role for a user.？《{&
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the us/*does */not have the role (aka not affected).</returns>
        public Task<bool> DeleteRoleForUserAsync(string user, string role, string domain = null)
        {
            return domain is null
                ? RemoveGroupingPolicyAsync(user, role)
                : RemoveGroupingPolicyAsync(user, role, domain);
        }

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        public bool DeleteRolesForUser(string user, string domain = null)
        {
            return domain is null
                ? RemoveFilteredGroupingPolicy(0, user)
                : RemoveFilteredGroupingPolicy(0, user, string.Empty, domain);
        }

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        public Task<bool> DeleteRolesForUserAsync(string user, string domain = null)
        {
            return domain is null
                ? RemoveFilteredGroupingPolicyAsync(0, user)
                : RemoveFilteredGroupingPolicyAsync(0, user, string.Empty, domain);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        public bool DeleteUser(string user)
        {
            bool groupResult = RemoveFilteredGroupingPolicy(0, user);
            bool result = RemoveFilteredPolicy(0, user);
            return groupResult || result;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        public async Task<bool> DeleteUserAsync(string user)
        {
            bool groupResult = await RemoveFilteredGroupingPolicyAsync(0, user);
            bool result = await RemoveFilteredPolicyAsync(0, user);
            return groupResult || result;
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Returns false if the role does not exist (aka not affected).</returns>
        public bool DeleteRole(string role)
        {
            bool groupResult = RemoveFilteredGroupingPolicy(1, role);
            bool result = RemoveFilteredPolicy(0, role);
            return groupResult || result;
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role"></param>
        public async Task<bool> DeleteRoleAsync(string role)
        {
            bool groupResult = await RemoveFilteredGroupingPolicyAsync(1, role);
            bool result = await RemoveFilteredPolicyAsync(0, role);
            return groupResult || result;
        }

        #endregion

        #region Get permissions

        /// <summary>
        /// Gets permissions for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<List<string>> GetPermissionsForUser(string user, string domain = null)
        {
            return domain is null
                ? GetFilteredPolicy(0, user)
                : GetFilteredPolicy(0, user, domain);
        }

        #endregion

        #region Has permissions

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermissionForUser(string user, List<string> permission)
        {
            return HasPermissionForUser(user, permission.ToArray());
        }

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermissionForUser(string user, params string[] permission)
        {
            var parameters = new List<string>
            {
                user
            };
            parameters.AddRange(permission);
            return HasPolicy(parameters);
        }

        #endregion

        #region Add permissions

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        public bool AddPermissionForUser(string user, List<string> permission)
        {
            return AddPermissionForUser(user, permission.ToArray());
        }

        /// <summary>
        /// Adds multiple permissions for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        public Task<bool> AddPermissionForUserAsync(string user, List<string> permission)
        {
            return AddPermissionForUserAsync(user, permission.ToArray());
        }

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public bool AddPermissionForUser(string user, params string[] permission)
        {
            var parameters = new[] {user}.Concat(permission);
            return AddPolicy(parameters.ToList());
        }

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public Task<bool> AddPermissionForUserAsync(string user, params string[] permission)
        {
            var parameters = new[] {user}.Concat(permission);
            return AddPolicyAsync(parameters.ToList());
        }

        #endregion

        #region Delete permissions

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public bool DeletePermission(List<string> permission)
        {
            return DeletePermission(permission.ToArray());
        }

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public Task<bool> DeletePermissionAsync(List<string> permission)
        {
            return DeletePermissionAsync(permission.ToArray());
        }

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public bool DeletePermission(params string[] permission)
        {
            return RemoveFilteredPolicy(1, permission);
        }

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public Task<bool> DeletePermissionAsync(params string[] permission)
        {
            return RemoveFilteredPolicyAsync(1, permission);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public bool DeletePermissionForUser(string user, List<string> permission)
        {
            return DeletePermissionForUser(user, permission.ToArray());
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public Task<bool> DeletePermissionForUserAsync(string user, List<string> permission)
        {
            return DeletePermissionForUserAsync(user, permission.ToArray());
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool DeletePermissionForUser(string user, params string[] permission)
        {
            var parameters = new[] {user}.Concat(permission);
            return RemovePolicy(parameters.ToList());
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public Task<bool> DeletePermissionForUserAsync(string user, params string[] permission)
        {
            var parameters = new[] {user}.Concat(permission);
            return RemovePolicyAsync(parameters.ToList());
        }

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role. 
        /// </summary>
        /// <param name="user">User or role</param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public bool DeletePermissionsForUser(string user)
        {
            return RemoveFilteredPolicy(0, user);
        }

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role. 
        /// </summary>
        /// <param name="user">User or role</param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public Task<bool> DeletePermissionsForUserAsync(string user)
        {
            return RemoveFilteredPolicyAsync(0, user);
        }

        #endregion

        #region Get implicit roles or users

        /// <summary>
        /// Gets implicit roles that a user has.
        /// Compared to GetRolesForUser(), this function retrieves indirect roles besides direct roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [Obsolete("This api will be removed in next mainline version. please use the another overwrite.")]
        public List<string> GetImplicitRolesForUser(string name, params string[] domain)
        {
            var roles = roleManager.GetRoles(name, domain);
            var res = new List<string>();
            res.AddRange(roles);
            res.AddRange(roles.SelectMany(x => GetImplicitRolesForUser(x, domain)));
            return res;
        }

        /// <summary>
        /// Gets implicit roles that a user has.
        /// Compared to GetRolesForUser(), this function retrieves indirect roles besides direct roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetImplicitRolesForUser(string name, string domain = null)
        {
            var roles = domain is null
                ? roleManager.GetRoles(name)
                : roleManager.GetRoles(name, domain);
            var result = new List<string>();
            result.AddRange(roles);
            result.AddRange(roles.SelectMany(x => GetImplicitRolesForUser(x, domain)));
            return result;
        }

        /// <summary>
        /// <para>Gets implicit permissions for a user or role.</para>
        /// <para>Compared to GetPermissionsForUser(), this function retrieves permissions for inherited roles.</para> 
        /// <para>For example:</para>
        /// <para>p, admin, data1, read</para>
        /// <para>p, alice, data2, read</para>
        /// <para>g, alice, admin </para>
        /// <para>GetPermissionsForUser("alice") can only get: [["alice", "data2", "read"]].</para>
        /// <para>But GetImplicitPermissionsForUser("alice") will get: [["admin", "data1", "read"], ["alice", "data2", "read"]].</para>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<List<string>> GetImplicitPermissionsForUser(string user, string domain = null)
        {
            var roles = new List<string> { user };
            roles.AddRange(GetImplicitRolesForUser(user, domain));
            var result = new List<List<string>>();
            foreach (string role in roles)
            {
                result.AddRange(GetPermissionsForUser(role, domain));
            }
            return result;
        }

        public IEnumerable<string> GetImplicitUsersForPermission(params string[] permission)
        {
            return GetImplicitUsersForPermission((IEnumerable<string>) permission);
        }

        public IEnumerable<string> GetImplicitUsersForPermission(IEnumerable<string> permissions)
        {
            List<string> policySubjects = GetAllSubjects();
            List<string> groupInherit = model.GetValuesForFieldInPolicyAllTypes("g", 1);
            List<string> groupSubjects = model.GetValuesForFieldInPolicyAllTypes("g", 0);
            return policySubjects.Concat(groupSubjects).Distinct().Except(groupInherit)
                .Where(subject => Enforce(new[]{ subject }.Concat(permissions).Cast<object>().ToArray()));
        }

        #endregion

        #region RBAC APIs with domains

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetRolesForUserInDomain(string name, string domain)
        {
            return model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType].RoleManager.GetRoles(name, domain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<List<string>> GetPermissionsForUserInDomain(string user, string domain)
        {
            return GetFilteredPolicy(0, user, domain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool AddRoleForUserInDomain(string user, string role, string domain)
        {
            return AddGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public Task<bool> AddRoleForUserInDomainAsync(string user, string role, string domain)
        {
            return AddGroupingPolicyAsync(user, role, domain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool DeleteRoleForUserInDomain(string user, string role, string domain)
        {
            return RemoveGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public Task<bool> DeleteRoleForUserInDomainAsync(string user, string role, string domain)
        {
            return RemoveGroupingPolicyAsync(user, role, domain);
        }

        #endregion
    }
}
