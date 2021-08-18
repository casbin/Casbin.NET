using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NetCasbin.Rbac
{
    public class DefaultRoleManager : IRoleManager
    {
        private readonly string _defaultDomain = string.Empty;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Role>> _allDomains = new();
        private readonly int _maxHierarchyLevel;

        private IEnumerable<string> _cachedAllDomains;
        private readonly ConcurrentDictionary<string, Role> _defaultRoles = new();

        public Func<string, string, bool> MatchingFunc { get; set; }
        public Func<string, string, bool> DomainMatchingFunc { get; set; }

        public bool HasPattern => MatchingFunc is not null;
        public bool HasDomainPattern => DomainMatchingFunc is not null;

        public DefaultRoleManager(int maxHierarchyLevel)
        {
            _allDomains[_defaultDomain] = _defaultRoles;
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
                throw new ArgumentException("Domain should be 1 parameter.");
            }
        }
        #endregion

        public IEnumerable<string> GetDomains(string name)
        {
            _cachedAllDomains ??= _allDomains.Keys;
            var domains = new HashSet<string>();
            foreach (string domain in _cachedAllDomains)
            {
                if (AnyRolesInDomain(name, domain))
                {
                    domains.Add(domain);
                }
            }
            return domains;
        }

        private IEnumerable<string> GetRoles(string name, string domain = null)
        {
            domain ??= _defaultDomain;
            if (HasDomainPattern is false)
            {
                return GetRolesInDomain(name, domain);
            }

            var roleNames = new List<string>();
            foreach (string matchDomain in GetPatternDomains(domain))
            {
                roleNames.AddRange(GetRolesInDomain(name, matchDomain));
            }

            return roleNames.Distinct();
        }

        private bool AnyRolesInDomain(string name, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return false;
                }
            }

            if (HasPattern is false)
            {
                return roles.ContainsKey(name);
            }

            foreach (KeyValuePair<string, Role> role in roles)
            {
                if (MatchingFunc(name, role.Key))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<string> GetRolesInDomain(string name, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return Enumerable.Empty<string>();
                }
            }

            if (HasPattern is false)
            {
                return roles.TryGetValue(name, out Role role)
                    ? role.GetRoles() : Enumerable.Empty<string>();
            }

            var rolesNames = new List<string>();
            foreach (KeyValuePair<string, Role> role in roles)
            {
                if (MatchingFunc(name, role.Key))
                {
                    rolesNames.AddRange(role.Value.GetRoles());
                }
            }
            return rolesNames;
        }

        private IEnumerable<string> GetUsers(string name, string domain = null)
        {
            domain ??= _defaultDomain;

            if (HasDomainPattern is false)
            {
                return GetUsersInDomain(name, domain);
            }

            var userNames = new List<string>();
            foreach (string matchDomain in GetPatternDomains(domain))
            {
                userNames.AddRange(GetUsersInDomain(name, matchDomain));
            }

            return userNames.Distinct();
        }

        private IEnumerable<string> GetUsersInDomain(string name, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return Enumerable.Empty<string>();
                }
            }

            var userNames = new List<string>();
            foreach (KeyValuePair<string, Role> role in roles)
            {
                if (role.Value.HasDirectRole(name, domain))
                {
                    userNames.Add(role.Key);
                }
            }
            return userNames;
        }

        private bool HasLink(string name1, string name2, string domain = null)
        {
            if (string.Equals(name1, name2))
            {
                return true;
            }

            domain ??= _defaultDomain;
            if (HasDomainPattern is false)
            {
                return HasLinkInDomain(name1, name2, domain);
            }

            foreach (string matchDomain in GetPatternDomains(domain))
            {
                bool hasLink = HasLinkInDomain(name1, name2, matchDomain);
                Debug.WriteLine("HasLink: {0}, {1}, {2} => {3}", name1, name2, matchDomain, hasLink);

                if (hasLink)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasLinkInDomain(string name1, string name2, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return false;
                }
            }

            if (HasPattern is false)
            {
                return roles.TryGetValue(name1, out Role role1) is not false
                       && role1.HasRole(name2, _maxHierarchyLevel, domain, null, DomainMatchingFunc);
            }

            foreach (KeyValuePair<string, Role> role in roles)
            {
                if (MatchingFunc(name1, role.Key) is false)
                {
                    continue;
                }

                if (role.Value.HasRole(name2, _maxHierarchyLevel, domain, MatchingFunc, DomainMatchingFunc))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            AddLinkInDomain(name1, name2, domain);
        }

        private void AddLinkInDomain(string name1, string name2, string domain)
        {
            ConcurrentDictionary<string, Role> roles = domain == _defaultDomain
                ? _defaultRoles
                : _allDomains.GetOrAdd(domain, new ConcurrentDictionary<string, Role>());

            Role role1 = roles.GetOrAdd(name1, new Role(name1, domain));
            Role role2 = roles.GetOrAdd(name2, new Role(name2, domain));

            role1.AddRole(role2);
        }

        private void DeleteLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            ConcurrentDictionary<string, Role> roles;
            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    ThrowHelper.ThrowOneOfNamesNotFoundException();
                    return;
                }
            }

            if (roles.TryGetValue(name1, out Role role1) is false
                || roles.TryGetValue(name2, out Role role2) is false)
            {
                ThrowHelper.ThrowOneOfNamesNotFoundException();
                return;
            }
            role1.DeleteRole(role2);
        }

        public virtual void Clear()
        {
            _allDomains.Clear();
            _defaultRoles.Clear();
            _allDomains[_defaultDomain] = _defaultRoles;
            _cachedAllDomains = null;
        }

        public void BuildRelationship(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            ConcurrentDictionary<string, Role> roles = domain == _defaultDomain
                ? _defaultRoles
                : _allDomains.GetOrAdd(domain, new ConcurrentDictionary<string, Role>());

            Role role1 = roles.GetOrAdd(name1, new Role(name1, domain));
            Role role2 = roles.GetOrAdd(name2, new Role(name2, domain));

            if (HasDomainPattern)
            {
                foreach (string matchDomain in GetPatternDomains(domain))
                {
                    if (domain == matchDomain)
                    {
                        continue;
                    }
                    BuildRelationshipInDomain(name1, name2, role1, role2, matchDomain);
                }
                _cachedAllDomains = null;
                return;
            }
            _cachedAllDomains = null;
            return;
        }

        private void BuildRelationshipInDomain(string name1, string name2, Role role1, Role role2, string domain)
        {
            ConcurrentDictionary<string, Role> roles = domain == _defaultDomain
                ? _defaultRoles
                : _allDomains.GetOrAdd(domain, new ConcurrentDictionary<string, Role>());

            if (HasPattern is false)
            {
                foreach (KeyValuePair<string, Role> role in roles)
                {
                    string name = role.Key;
                    if (name == name1)
                    {
                        role1.AddRole(role.Value);
                    }
                    if (name == name2)
                    {
                        role2.AddRole(role.Value);
                    }
                }
                return;
            }

            foreach (KeyValuePair<string, Role> role in roles)
            {
                string name = role.Key;
                if (name == name1 || name == name2)
                {
                    return;
                }
                if (MatchingFunc(name, name1) || MatchingFunc(name1, name))
                {
                    role.Value.AddRole(role1);
                }
                if (MatchingFunc(name, name2) || MatchingFunc(name2, name))
                {
                    role2.AddRole(role.Value);
                }
            }
        }

        private IEnumerable<string> GetPatternDomains(string domain)
        {
            List<string> matchDomains = new() { domain };
            _cachedAllDomains ??= _allDomains.Keys;
            if (HasDomainPattern)
            {
                matchDomains.AddRange(_cachedAllDomains.Where(key =>
                    DomainMatchingFunc(domain, key) && key != domain));
            }
            return matchDomains;
        }
    }
}
