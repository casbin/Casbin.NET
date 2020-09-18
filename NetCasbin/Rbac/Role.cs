using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCasbin.Rbac
{
    /// <summary>
    /// Role represents the data structure for a role in RBAC.
    /// </summary>
    public class Role
    {
        private readonly Lazy<Dictionary<string, Role>> _roles = new Lazy<Dictionary<string, Role>>();

        public Role(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddRole(Role role)
        {
            if (_roles.IsValueCreated is false)
            {
                _roles.Value.Add(role.Name, role);
            }

            if (_roles.Value.ContainsKey(role.Name))
            {
                return;
            }

            _roles.Value.Add(role.Name, role);
        }

        public void DeleteRole(Role role)
        {
            if (_roles.IsValueCreated is false)
            {
                return;
            }

            if (_roles.Value.ContainsKey(role.Name))
            {
                _roles.Value.Remove(role.Name);
            }
        }

        public bool HasRole(string roleName, int hierarchyLevel)
        {
            if (Name == roleName)
            {
                return true;
            }

            if (hierarchyLevel <= 0)
            {
                return false;
            }

            if (_roles.IsValueCreated is false)
            {
                return false;
            }

            return _roles.Value.Values.Any(role => role.HasRole(roleName, hierarchyLevel - 1));
        }

        public bool HasDirectRole(string roleName)
        {
            return _roles.IsValueCreated is not false && _roles.Value.ContainsKey(roleName);
        }

        public List<string> GetRoles()
        {
            return _roles.Value.Select(x => x.Key).ToList();
        }

        public override string ToString()
        {
            return $"{Name}{string.Join(",", _roles.Value)}";
        }
    }
}
