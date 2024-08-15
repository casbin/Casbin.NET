﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Casbin.Rbac
{
    /// <summary>
    /// Role represents the data structure for a role in RBAC.
    /// </summary>
    public class Role
    {
        private readonly Lazy<ConcurrentDictionary<string, Role>> _roles = new();

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
            _roles.Value.TryAdd(role.Name, role);
        }

        public void DeleteRole(Role role)
        {
            if (_roles.IsValueCreated is false)
            {
                return;
            }
            _roles.Value.TryRemove(role.Name, out _);
        }

        public bool HasRole(string name, int hierarchyLevel, Func<string, string, bool> matchingFunc = null)
        {
            if (HasDirectRole(name, matchingFunc))
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

            return _roles.Value.Values.Any(role =>
                role.HasRole(name, hierarchyLevel - 1));
        }

        public bool HasDirectRole(string name, Func<string, string, bool> matchingFunc = null)
        {
            if (_roles.IsValueCreated is false)
            {
                return false;
            }

            if (matchingFunc is null)
            {
                return _roles.Value.ContainsKey(name);
            }


            foreach (var role in _roles.Value.Values)
            {
                if (name == role.Name || matchingFunc(role.Name, name) && role.Name != name)
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
    }
}
