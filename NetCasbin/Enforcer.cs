using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCasbin
{
    /// <summary>
    /// Enforcer = ManagementEnforcer + RBAC API.
    /// </summary>
    public class Enforcer : ManagementEnforcer
    {

        public Enforcer() : this("", "")
        {
        }

        public Enforcer(string modelPath, string policyFile) : this(modelPath, new DefaultFileAdapter(policyFile))
        {
        }

        public Enforcer(string modelPath, IAdapter adapter) : this(NewModel(modelPath, ""), adapter)
        {
            this.modelPath = modelPath;
        }

        public Enforcer(Model.Model m, IAdapter adapter)
        {
            this.adapter = adapter;
            watcher = null;

            model = m;
            fm = FunctionMap.LoadFunctionMap();

            Initialize();
            LoadPolicy();
        }

        public Enforcer(Model.Model m) :
            this(m, null)
        {
        }

        public Enforcer(string modelPath) :
            this(modelPath, "")
        {
        }

        public Enforcer(string modelPath, string policyFile, bool enableLog) : this(modelPath, new DefaultFileAdapter(policyFile))
        {
        }

        public List<string> GetRolesForUser(string name)
        {
            return model.Model["g"]["g"].RoleManager.GetRoles(name);
        }

        public List<string> GetUsersForRole(string name)
        {
            return model.Model["g"]["g"].RoleManager.GetUsers(name);
        }

        public List<string> GetUsersForRoles(string[] names)
        {
            var userIds = new List<string>();
            foreach (var name in names)
                userIds.AddRange(model.Model["g"]["g"].RoleManager.GetUsers(name));
            return userIds;
        }

        public bool HasRoleForUser(string name, string role)
        {
            var roles = GetRolesForUser(name);

            var hasRole = false;
            foreach (var r in roles)
            {
                if (r.Equals(role))
                {
                    hasRole = true;
                    break;
                }
            }

            return hasRole;
        }

        public bool AddRoleForUser(string user, string role)
        {
            return AddGroupingPolicy(user, role);
        }

        public bool DeleteRoleForUser(string user, string role)
        {
            return RemoveGroupingPolicy(user, role);
        }

        public bool DeleteRolesForUser(string user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        /// <summary>
        /// DeleteUser deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns> Returns false if the user does not exist (aka not affected).</returns>
        public bool DeleteUser(string user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        /// <summary>
        /// DeleteRole deletes a role.
        /// </summary>
        /// <param name="role"></param>
        public void DeleteRole(string role)
        {
            RemoveFilteredGroupingPolicy(1, role);
            RemoveFilteredPolicy(0, role);
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

        public bool DeletePermission(List<string> permission)
        {
            return DeletePermission(permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// AddPermissionForUser adds a permission for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
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

        public bool AddPermissionForUser(string user, List<string> permission)
        {
            return AddPermissionForUser(user, permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
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
        /// <param name="user">user or role </param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public bool DeletePermissionForUser(string user, List<string> permission)
        {
            return DeletePermissionForUser(user, permission.ToArray() ?? new string[0]);
        }

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role. 
        /// </summary>
        /// <param name="user">user or role </param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public bool DeletePermissionsForUser(string user)
        {
            return RemoveFilteredPolicy(0, user);
        }

        /// <summary>
        /// GetPermissionsForUser gets permissions for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
        /// <returns></returns>
        public List<List<string>> GetPermissionsForUser(string user)
        {
            return GetFilteredPolicy(0, user);
        }

        /// <summary>
        ///  HasPermissionForUser determines whether a user has a permission.
        /// </summary>
        /// <param name="user"></param>
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
        /// HasPermissionForUser determines whether a user has a permission.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermissionForUser(string user, List<string> permission)
        {
            return HasPermissionForUser(user, permission.ToArray() ?? new string[0]);
        }

        public List<string> GetRolesForUserInDomain(string name, string domain)
        {
            return model.Model["g"]["g"].RoleManager.GetRoles(name, domain);
        }

        public List<List<string>> GetPermissionsForUserInDomain(string user, string domain)
        {
            return GetFilteredPolicy(0, user, domain);
        }

        public bool AddRoleForUserInDomain(string user, string role, string domain)
        {
            return AddGroupingPolicy(user, role, domain);
        }

        public bool DeleteRoleForUserInDomain(string user, string role, string domain)
        {
            return RemoveGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// GetImplicitRolesForUser gets implicit roles that a user has.
        /// Compared to GetRolesForUser(), this function retrieves indirect roles besides direct roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetImplicitRolesForUser(string name, params string[] domain)
        {
            var roles = rm.GetRoles(name, domain);
            var res = new List<string>();
            res.AddRange(roles);
            res.AddRange(roles.SelectMany(x => GetImplicitRolesForUser(x, domain)));
            return res;
        }
        /// <summary>
        /// <para>gets implicit permissions for a user or role.</para>
        /// <para>Compared to GetPermissionsForUser(), this function retrieves permissions for inherited roles.</para> 
        /// <para>For example:</para>
        /// <para>p, admin, data1, read</para>
        /// <para>p, alice, data2, read</para>
        /// <para>g, alice, admin </para>
        /// <para>GetPermissionsForUser("alice") can only get: [["alice", "data2", "read"]].</para>
        /// <para>But GetImplicitPermissionsForUser("alice") will get: [["admin", "data1", "read"], ["alice", "data2", "read"]].</para>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<List<string>> GetImplicitPermissionsForUser(string user)
        {
            var roles = new List<string>
            {
                user
            };
            roles.AddRange(GetImplicitRolesForUser(user));
            var res = new List<List<string>>();
            foreach (var n in roles)
            {
                res.AddRange(GetPermissionsForUser(n));
            }
            return res;
        }

    }
}
