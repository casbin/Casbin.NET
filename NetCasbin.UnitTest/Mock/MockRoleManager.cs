using System;
using System.Collections.Generic;
using System.Linq;
using NetCasbin.Rbac;

namespace NetCasbin.UnitTest.Mock
{
    public class MockCustomRoleManager : IRoleManager
    {
        public Func<string, string, bool> MatchingFunc { get; set; }
        public Func<string, string, bool> DomainMatchingFunc { get; set; }
        public bool HasPattern => false;
        public bool HasDomainPattern => false;

        public List<string> GetRoles(string name, params string[] domain) => null;
        public List<string> GetUsers(string name, params string[] domain) => null;
        public IEnumerable<string> GetDomains(string name)
        {
            return Enumerable.Empty<string>();
        }

        public bool HasLink(string name1, string name2, params string[] domain)
        {
            if (name1.Equals("alice") && name2.Equals("alice"))
            {
                return true;
            }

            if (name1.Equals("alice") && name2.Equals("data2_admin"))
            {
                return true;
            }

            if (name1.Equals("bob") && name2.Equals("bob"))
            {
                return true;
            }

            return false;
        }

        public void AddLink(string name1, string name2, params string[] domain)
        {
        }

        public void DeleteLink(string name1, string name2, params string[] domain)
        {
        }

        public void Clear()
        {
        }

        public void BuildRelationship(string name1, string name2, string domain = null)
        {
        }
    }
}
