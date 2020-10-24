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

        public Enforcer(string modelPath, string policyFile) : this(modelPath, new DefaultFileAdapter(policyFile))
        {
        }

        public Enforcer(string modelPath, IAdapter adapter) : this(NewModel(modelPath, string.Empty), adapter)
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

        public Enforcer(Model.Model m) :
            this(m, null)
        {
        }

        public Enforcer(string modelPath) :
            this(modelPath, string.Empty)
        {
        }

        public Enforcer(string modelPath, string policyFile, bool enableLog) : this(modelPath, new DefaultFileAdapter(policyFile))
        {
        }

        /// <summary>
        /// Gets the roles that a user has.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<string> GetRolesForUser(string name)
        {
            return model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType].RoleManager.GetRoles(name);
        }

        /// <summary>
        /// Gets the users that has a role.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<string> GetUsersForRole(string name)
        {
            return model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType].RoleManager.GetUsers(name);
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
                userIds.AddRange(model.Model[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType].RoleManager.GetUsers(name));
            return userIds;
        }

        /// <summary>
        /// Determines whether a user has a role.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool HasRoleForUser(string name, string role)
        {
            var roles = GetRolesForUser(name);

            bool hasRole = false;
            foreach (string r in roles)
            {
                if (r.Equals(role))
                {
                    hasRole = true;
                    break;
                }
            }

            return hasRole;
        }

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public bool AddRoleForUser(string user, string role)
        {
            return AddGroupingPolicy(user, role);
        }

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public Task<bool> AddRoleForUserAsync(string user, string role)
        {
            return AddGroupingPolicyAsync(user, role);
        }

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>Returns false if the user does not have the role (aka not affected).</returns>
        public bool DeleteRoleForUser(string user, string role)
        {
            return RemoveGroupingPolicy(user, role);
        }

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>Returns false if the user does not have the role (aka not affected).</returns>
        public Task<bool> DeleteRoleForUserAsync(string user, string role)
        {
            return RemoveGroupingPolicyAsync(user, role);
        }

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        public bool DeleteRolesForUser(string user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        public Task<bool> DeleteRolesForUserAsync(string user)
        {
            return RemoveFilteredGroupingPolicyAsync(0, user);
        }

        /// <summary>
        /// DeleteUser deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        public bool DeleteUser(string user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        /// <summary>
        /// DeleteUser deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        public Task<bool> DeleteUserAsync(string user)
        {
            return RemoveFilteredGroupingPolicyAsync(0, user);
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role"></param>
        public void DeleteRole(string role)
        {
            RemoveFilteredGroupingPolicy(1, role);
            RemoveFilteredPolicy(0, role);
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role"></param>
        public async Task DeleteRoleAsync(string role)
        {
            await RemoveFilteredGroupingPolicyAsync(1, role);
            await RemoveFilteredPolicyAsync(0, role);
        }

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
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        public bool AddPermissionForUser(string user, List<string> permission)
        {
            return AddPermissionForUser(user, permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// Adds multiple permissions for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        public Task<bool> AddPermissionForUserAsync(string user, List<string> permission)
        {
            return AddPermissionForUserAsync(user, permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public bool AddPermissionForUser(string user, params string[] permission)
        {
            var parameters = new List<string>
            {
                user
            };
            parameters.AddRange(permission);
            return AddPolicy(parameters);
        }

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public Task<bool> AddPermissionForUserAsync(string user, params string[] permission)
        {
            var parameters = new List<string>
            {
                user
            };
            parameters.AddRange(permission);
            return AddPolicyAsync(parameters);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public bool DeletePermissionForUser(string user, List<string> permission)
        {
            return DeletePermissionForUser(user, permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public Task<bool> DeletePermissionForUserAsync(string user, List<string> permission)
        {
            return DeletePermissionForUserAsync(user, permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool DeletePermissionForUser(string user, params string[] permission)
        {
            var parameters = new List<string>
            {
                user
            };
            parameters.AddRange(permission);
            return RemovePolicy(parameters);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public Task<bool> DeletePermissionForUserAsync(string user, params string[] permission)
        {
            var parameters = new List<string>
            {
                user
            };
            parameters.AddRange(permission);
            return RemovePolicyAsync(parameters);
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

        /// <summary>
        /// Gets permissions for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <returns></returns>
        public List<List<string>> GetPermissionsForUser(string user)
        {
            return GetFilteredPolicy(0, user);
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

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermissionForUser(string user, List<string> permission)
        {
            return HasPermissionForUser(user, permission.ToArray() ?? new string[0]);
        }

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

        /// <summary>
        /// Gets implicit roles that a user has.
        /// Compared to GetRolesForUser(), this function retrieves indirect roles besides direct roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetImplicitRolesForUser(string name, params string[] domain)
        {
            var roles = roleManager.GetRoles(name, domain);
            var res = new List<string>();
            res.AddRange(roles);
            res.AddRange(roles.SelectMany(x => GetImplicitRolesForUser(x, domain)));
            return res;
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
        /// <param name="user">User or role</param>
        /// <returns></returns>
        public List<List<string>> GetImplicitPermissionsForUser(string user)
        {
            var roles = new List<string>
            {
                user
            };
            roles.AddRange(GetImplicitRolesForUser(user));
            var res = new List<List<string>>();
            foreach (string n in roles)
            {
                res.AddRange(GetPermissionsForUser(n));
            }
            return res;
        }

        public IEnumerable<string> GetImplicitUsersForPermission(params string[] permission)
        {
            return GetImplicitUsersForPermission((IEnumerable<string>) permission);
        }

        public IEnumerable<string> GetImplicitUsersForPermission(IEnumerable<string> permissions)
        {
            var policySubjects = GetAllSubjects();
            var groupInherit = model.GetValuesForFieldInPolicyAllTypes("g", 1);
            var groupSubjects = model.GetValuesForFieldInPolicyAllTypes("g", 0);
            return policySubjects.Concat(groupSubjects).Distinct()
                .Where(subject => Enforce(new[]{ subject }.Concat(permissions).Cast<object>().ToArray()))
                .Except(groupInherit);
        }
    }
}
