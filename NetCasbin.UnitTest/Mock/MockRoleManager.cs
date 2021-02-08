using System;
using System.Collections.Generic;
using Casbin.Rbac;

namespace Casbin.UnitTests.Mock
{
    public class MockCustomRoleManager : IRoleManager
    {
        public Func<string, string, bool> MatchingFunc { get; set; }
        public Func<string, string, bool> DomainMatchingFunc { get; set; }
        public bool HasPattern { get; } = false;
        public bool HasDomainPattern { get; } = false;

        public List<string> GetRoles(string name, params string[] domain) => null;
        public List<string> GetUsers(string name, params string[] domain) => null;

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
    }
}
