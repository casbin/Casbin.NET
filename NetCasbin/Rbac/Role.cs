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
        private readonly Lazy<Dictionary<string, Role>> _roles = new();

        public Role(string name)
        {
            Name = name;
        }

        public Role(string name, string domain)
        {
            Name = name;
            Domain = domain;
        }

        public string Name { get; }

        public string Domain { get; } = string.Empty;

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

        public bool HasRole(string name, int hierarchyLevel, string domain,
            Func<string, string, bool> matchingFunc = null,
            Func<string, string, bool> domainMatchingFunc = null)
        {
            if (HasDirectRole(name, domain, matchingFunc, domainMatchingFunc))
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

            return _roles.Value.Values.Any(role => role.HasRole(name, hierarchyLevel - 1, domain, matchingFunc, domainMatchingFunc));
        }

        public bool HasDirectRole(string name, string domain,
            Func<string, string, bool> matchingFunc = null,
            Func<string, string, bool> domainMatchingFunc = null)
        {
            if (_roles.IsValueCreated is false)
            {
                return false;
            }

            if (matchingFunc is null)
            {
                if (_roles.Value.TryGetValue(name, out Role _) is false)
                {
                    return false;
                }

                // TODO: unused domainMatchingFunc

                return true;
            }

            foreach (Role role in _roles.Value.Values)
            {
                if (domainMatchingFunc is not null && domainMatchingFunc(role.Domain, domain) is false)
                {
                    return false;
                }

                if (name == role.Name)
                {
                    return true;
                }

                if (role.Name != name && matchingFunc(role.Name, name))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<string> GetRoles()
        {
            return _roles.IsValueCreated ? _roles.Value.Keys : Enumerable.Empty<string>();
        }

        public override string ToString()
        {
            return $"{Name}{string.Join(",", _roles.Value)}";
        }
    }
}
