using System.Collections.Generic;
using System.Linq;

namespace NetCasbin.Rbac
{
    /// <summary>
    /// Role represents the data structure for a role in RBAC.
    /// </summary>
    public class Role
    {
        private string _name;

        private readonly Dictionary<string, Role> _roles = new Dictionary<string, Role>();

        public Role(string name)
        {
            _name = name;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public void AddRole(Role role)
        {
            if (_roles.ContainsKey(role.Name))
            {
                return;
            }

            _roles.Add(role.Name, role);
        }

        public void DeleteRole(Role role)
        {
            if (_roles.ContainsKey(role._name))
            {
                _roles.Remove(role.Name);
            }
        }

        public bool HasRole(string roleName, int hierarchyLevel)
        {
            if (_name == roleName)
            {
                return true;
            }

            if (hierarchyLevel <= 0)
            {
                return false;
            }

            foreach (var role in _roles.Values)
            {
                if (role.HasRole(roleName, hierarchyLevel - 1))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasDirectRole(string roleName)
        {
            return _roles.ContainsKey(roleName);
        }

        public List<string> GetRoles()
        {
            return _roles.Select(x => x.Key).ToList();
        }

        public override string ToString()
        {
            return $"{_name}{string.Join(",", _roles)}";
        }
    }
}
