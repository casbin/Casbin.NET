using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCasbin
{
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

        public Enforcer(Model m, IAdapter adapter)
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

        public Enforcer(Model m) :
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

        public Boolean DeleteUser(String user)
        {
            return RemoveFilteredGroupingPolicy(0, user);
        }

        public void DeleteRole(String role)
        {
            RemoveFilteredGroupingPolicy(1, role);
            RemoveFilteredPolicy(0, role);
        }

        public Boolean DeletePermission(params string[] permission)
        {
            return RemoveFilteredPolicy(1, permission);
        }

        public Boolean DeletePermission(List<String> permission)
        {
            return DeletePermission(permission.ToArray() ?? new String[0]);
        }

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

        public Boolean DeletePermissionForUser(String user, params string[] permission)
        {
            List<String> parameters = new List<string>();
            parameters.Add(user);
            parameters.AddRange(permission);
            return RemovePolicy(parameters);
        }

        public Boolean DeletePermissionForUser(String user, List<String> permission)
        {
            return DeletePermissionForUser(user, permission.ToArray()?? new String[0]);
        }

        public Boolean deletePermissionsForUser(String user)
        {
            return RemoveFilteredPolicy(0, user);
        }

        public List<List<String>> GetPermissionsForUser(String user)
        {
            return GetFilteredPolicy(0, user);
        }

        public Boolean HasPermissionForUser(String user, params string[] permission)
        {
            List<String> parameters = new List<string>();
            parameters.Add(user);
            parameters.AddRange(permission);
            return HasPolicy(permission);
        }

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

        public List<String> GetImplicitRolesForUser(String name,params string[] domain)
        {
            List<String> roles = this.rm.GetRoles(name, domain);
            List<String> res = new List<string>();
            res.AddRange(roles);
            res.AddRange(roles.SelectMany(x => this.GetImplicitRolesForUser(x, domain)));
            return res;
        }

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
