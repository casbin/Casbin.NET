using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCasbin.Rabc
{
    public class Role
    {
        private string _name;

        private Dictionary<string, Role> _roles = new Dictionary<string, Role>();

        public Role(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get => this._name;

            set
            {
                this._name = value;
            }
        }

        public void AddRole(Role role)
        {
            if (this._roles.ContainsKey(role.Name))
            {
                return;
            }

            this._roles.Add(role.Name, role);
        }

        public void DeleteRole(Role role)
        {
            if (this._roles.ContainsKey(role._name))
            {
                this._roles.Remove(role.Name);
            }
        }

        public bool HasRole(string roleName, int hierarchyLevel)
        {
            if (this._name == roleName)
            {
                return true;
            }

            if (hierarchyLevel <= 0)
            {
                return false;
            }

            foreach (Role role in this._roles.Values)
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
            return this._roles.ContainsKey(roleName);
        }

        public List<string> GetRoles()
        {
            return this._roles.Select(x => x.Key).ToList();
        }

        public override string ToString()
        {
            return $"{_name}{string.Join(",", this._roles)}";
        }
    }
}
