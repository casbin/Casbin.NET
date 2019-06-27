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

        public Enforcer(String modelPath, String policyFile) : this(modelPath, new DefaultFileAdapter(policyFile))
        {
        }

        public Enforcer(String modelPath, IAdapter adapter) : this(NewModel(modelPath, ""), adapter)
        {
            base.modelPath = modelPath;
        }

        public Enforcer(Model.Model m, IAdapter adapter)
        {
            this.adapter = adapter;
            this.watcher = null;

            model = m;
            fm = FunctionMap.LoadFunctionMap();

            Initialize();

            if (this.adapter != null)
            {
                LoadPolicy();
            }
        }

        public Enforcer(Model.Model m) :
            this(m, null)
        {
        }

        public Enforcer(String modelPath) :
            this(modelPath, "")
        {
        }

        public Enforcer(String modelPath, String policyFile, Boolean enableLog) : this(modelPath, new DefaultFileAdapter(policyFile))
        {
        }

        public List<String> GetRolesForUser(String name)
        {
            return model.Model["g"]["g"].RM.GetRoles(name);
        }

        public List<String> GetUsersForRole(String name)
        {
            return model.Model["g"]["g"].RM.GetUsers(name);
        }

        public Boolean HasRoleForUser(String name, String role)
        {
            List<String> roles = GetRolesForUser(name);

            Boolean hasRole = false;
            foreach (String r in roles)
            {
                if (r.Equals(role))
                {
                    hasRole = true;
                    break;
                }
            }

            return hasRole;
        }

        public Boolean AddRoleForUser(String user, String role)
        {
            return AddGroupingPolicy(user, role);
        }

        public Boolean DeleteRoleForUser(String user, String role)
        {
            return RemoveGroupingPolicy(user, role);
        }

        public Boolean DeleteRolesForUser(String user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        /// <summary>
        /// DeleteUser deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns> Returns false if the user does not exist (aka not affected).</returns>
        public Boolean DeleteUser(String user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        /// <summary>
        /// DeleteRole deletes a role.
        /// </summary>
        /// <param name="role"></param>
        public void DeleteRole(String role)
        {
            RemoveFilteredGroupingPolicy(1, role);
            RemoveFilteredPolicy(0, role);
        }

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public Boolean DeletePermission(params string[] permission)
        {
            return RemoveFilteredPolicy(1, permission);
        }

        public Boolean DeletePermission(List<String> permission)
        {
            return DeletePermission(permission.ToArray() ?? new String[0]);
        }

        /// <summary>
        /// AddPermissionForUser adds a permission for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public Boolean AddPermissionForUser(String user, params string[] permission)
        {
            List<String> parameters = new List<string>();
            parameters.Add(user);
            parameters.AddRange(permission);
            return AddPolicy(parameters);
        }

        public Boolean AddPermissionForUser(String user, List<String> permission)
        {
            return AddPermissionForUser(user, permission.ToArray() ?? new String[0]);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public Boolean DeletePermissionForUser(String user, params string[] permission)
        {
            List<String> parameters = new List<string>();
            parameters.Add(user);
            parameters.AddRange(permission);
            return RemovePolicy(parameters);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public Boolean DeletePermissionForUser(String user, List<String> permission)
        {
            return DeletePermissionForUser(user, permission.ToArray() ?? new String[0]);
        }

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role. 
        /// </summary>
        /// <param name="user">user or role </param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public Boolean DeletePermissionsForUser(String user)
        {
            return RemoveFilteredPolicy(0, user);
        }

        /// <summary>
        /// GetPermissionsForUser gets permissions for a user or role.
        /// </summary>
        /// <param name="user">user or role </param>
        /// <returns></returns>
        public List<List<String>> GetPermissionsForUser(String user)
        {
            return GetFilteredPolicy(0, user);
        }

        /// <summary>
        ///  HasPermissionForUser determines whether a user has a permission.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public Boolean HasPermissionForUser(String user, params string[] permission)
        {
            List<String> parameters = new List<string>
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
        public Boolean HasPermissionForUser(String user, List<String> permission)
        {
            return HasPermissionForUser(user, permission.ToArray() ?? new String[0]);
        }

        public List<String> GetRolesForUserInDomain(String name, String domain)
        {
            return model.Model["g"]["g"].RM.GetRoles(name, domain);
        }

        public List<List<String>> GetPermissionsForUserInDomain(String user, String domain)
        {
            return GetFilteredPolicy(0, user, domain);
        }

        public Boolean AddRoleForUserInDomain(String user, String role, String domain)
        {
            return AddGroupingPolicy(user, role, domain);
        }

        public Boolean DeleteRoleForUserInDomain(String user, String role, String domain)
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
        public List<String> GetImplicitRolesForUser(String name, params string[] domain)
        {
            List<String> roles = this.rm.GetRoles(name, domain);
            List<String> res = new List<string>();
            res.AddRange(roles);
            res.AddRange(roles.SelectMany(x => this.GetImplicitRolesForUser(x, domain)));
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
        public List<List<String>> GetImplicitPermissionsForUser(String user)
        {
            List<String> roles = new List<string>();
            roles.Add(user);
            roles.AddRange(this.GetImplicitRolesForUser(user));
            List<List<String>> res = new List<List<string>>();
            foreach (String n in roles)
            {
                res.AddRange(this.GetPermissionsForUser(n));
            }
            return res;
        }

    }
}
