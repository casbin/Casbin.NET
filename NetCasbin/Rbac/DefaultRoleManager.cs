using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NetCasbin.Rbac
{
    public class DefaultRoleManager : IRoleManager
    {
        private readonly string _defaultDomain = "casbin::default";

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Role>> _allDomains = new();
        private readonly int _maxHierarchyLevel;

        public Func<string, string, bool> MatchingFunc { get; set; }
        public Func<string, string, bool> DomainMatchingFunc { get; set; }

        public bool HasPattern => MatchingFunc is not null;
        public bool HasDomainPattern => DomainMatchingFunc is not null;

        public DefaultRoleManager(int maxHierarchyLevel)
        {
            _allDomains.TryAdd(_defaultDomain, new ConcurrentDictionary<string, Role>());
            _maxHierarchyLevel = maxHierarchyLevel;
        }

        #region Obsoleted APIs
        public virtual List<string> GetRoles(string name, params string[] domain)
        {
            CheckDomainArgument(domain);
            IEnumerable<string> roles = domain.Length is 0
                ? GetRoles(name)
                : GetRoles(name, domain[0]);
            return roles as List<string> ?? roles.ToList();
        }

        public virtual List<string> GetUsers(string name, params string[] domain)
        {
            CheckDomainArgument(domain);
            IEnumerable<string> users =  domain.Length is 0
                ? GetUsers(name)
                : GetUsers(name, domain[0]);
            return users as List<string> ?? users.ToList();
        }

        public virtual bool HasLink(string name1, string name2, params string[] domain)
        {
            CheckDomainArgument(domain);
            return domain.Length is 0
                ? HasLink(name1, name2)
                : HasLink(name1, name2, domain[0]);
        }

        public virtual void AddLink(string name1, string name2, params string[] domain)
        {
            CheckDomainArgument(domain);
            if (domain.Length is 0)
            {
                AddLink(name1, name2);
            }
            else
            {
                AddLink(name1, name2, domain[0]);
            }
        }

        public virtual void DeleteLink(string name1, string name2, params string[] domain)
        {
            CheckDomainArgument(domain);
            if (domain.Length is 0)
            {
                DeleteLink(name1, name2);
            }
            else
            {
                DeleteLink(name1, name2, domain[0]);
            }
        }

        private void CheckDomainArgument(string[] domain)
        {
            if (domain.Length > 1)
            {
                throw new ArgumentException(" Domain should be 1 parameter.");
            }
        }
        #endregion

        private IEnumerable<string> GetRoles(string name, string domain = null)
        {
            domain ??= _defaultDomain;
            ConcurrentDictionary<string, Role> roles;
            if (HasPattern || HasDomainPattern)
            {
                roles = GenerateTempRoles(domain);
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return Enumerable.Empty<string>();
                }
            }

            Func<string, string, bool> matchingFunc = MatchingFunc;
            return HasRole(roles, name, matchingFunc)
                ? CreateRole(roles, name, matchingFunc).GetRoles()
                : Enumerable.Empty<string>();
        }

        private IEnumerable<string> GetUsers(string name, string domain = null)
        {
            domain ??= _defaultDomain;
            ConcurrentDictionary<string, Role> roles;
            if (HasPattern || HasDomainPattern)
            {
                roles = GenerateTempRoles(domain);
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    // ThrowHelper.ThrowNameNotFoundException();
                    // return null;
                    return Enumerable.Empty<string>(); // Avoid breaking change
                }
            }

            if (HasRole(roles, name, MatchingFunc) is false)
            {
                // ThrowHelper.ThrowNameNotFoundException();
                return Enumerable.Empty<string>(); // Avoid breaking change
            }

            return roles.Values
                .Where(role => role.HasDirectRole(name))
                .Select(role => role.Name);
        }

        private bool HasLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            if (string.Equals(name1, name2))
            {
                return true;
            }

            ConcurrentDictionary<string, Role> roles;
            if (HasPattern || HasDomainPattern)
            {
                roles = GenerateTempRoles(domain);
            }
            else
            {
                roles = _allDomains.GetOrAdd(domain, new ConcurrentDictionary<string, Role>());
            }

            Func<string, string, bool> matchingFunc = MatchingFunc;
            if (HasRole(roles, name1, matchingFunc) is false
                || HasRole(roles, name1, matchingFunc) is false)
            {
                return false;
            }

            Role role = CreateRole(roles, name1, matchingFunc);
            return role.HasRole(name2, _maxHierarchyLevel);
        }

        private void AddLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            var roles = _allDomains.GetOrAdd(domain,
                new ConcurrentDictionary<string, Role>());

            Role role1 = roles.GetOrAdd(name1, new Role(name1));
            Role role2 = roles.GetOrAdd(name2, new Role(name2));
            role1.AddRole(role2);
        }

        private void DeleteLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            var roles = _allDomains.GetOrAdd(domain,
                new ConcurrentDictionary<string, Role>());

            if (roles.ContainsKey(name1) is false
                || roles.ContainsKey(name2) is false)
            {
                ThrowHelper.ThrowOneOfNamesNotFoundException();
            }

            Role role1 = roles.GetOrAdd(name1, new Role(name1));
            Role role2 = roles.GetOrAdd(name2, new Role(name2));
            role1.DeleteRole(role2);
        }

        public virtual void Clear()
        {
            _allDomains.Clear();
        }

        private static bool HasRole(ConcurrentDictionary<string, Role> roles, string name,
            Func<string, string, bool> matchingFunc = null)
        {
            if (matchingFunc is null)
            {
                return roles.ContainsKey(name);
            }

            ICollection<string> keys = roles.Keys;
            return keys.Any(key => matchingFunc(name, key));
        }

        private static Role CreateRole(ConcurrentDictionary<string, Role> roles, string name,
            Func<string, string, bool> matchingFunc = null)
        {
            Role role = roles.GetOrAdd(name, new Role(name));

            if (matchingFunc is null)
            {
                return role;
            }

            ICollection<string> keys = roles.Keys;
            foreach (string key in keys)
            {
                if (name == key || matchingFunc(name, key) is false)
                {
                    continue;
                }

                Role matchingRole = roles.GetOrAdd(key, new Role(key));
                role.AddRole(matchingRole);
            }
            return role;
        }

        private ConcurrentDictionary<string, Role> GenerateTempRoles(string domain)
        {
            _allDomains.TryAdd(domain, new ConcurrentDictionary<string, Role>());

            List<string> patternDomains = new() { domain };
            if (HasDomainPattern)
            {
                Func<string, string, bool> domainMatchingFunc = DomainMatchingFunc;
                ICollection<string> keys = _allDomains.Keys;
                patternDomains.AddRange(keys.Where(key =>
                    domainMatchingFunc(domain, key)));
            }

            Func<string, string, bool> matchingFunc = MatchingFunc;
            ConcurrentDictionary<string, Role> roles = new();
            foreach (string patternDomain in patternDomains)
            {
                var rolesInDomain = _allDomains.GetOrAdd(
                    patternDomain, new ConcurrentDictionary<string, Role>());
                foreach (KeyValuePair<string, Role> keyValue in rolesInDomain)
                {
                    Role role2 = keyValue.Value;
                    Role role1 = CreateRole(roles, role2.Name, matchingFunc);
                    foreach (string roleName in role2.GetRoles())
                    {
                        role1.AddRole(CreateRole(roles, roleName, matchingFunc));
                    }
                }
            }

            return roles;
        }

    }
}
