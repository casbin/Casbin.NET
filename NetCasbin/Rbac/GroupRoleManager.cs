using System;
using System.Collections.Generic;

namespace NetCasbin.Rbac
{
    public class GroupRoleManager : DefaultRoleManager
    {
        public GroupRoleManager(int maxHierarchyLevel) : base(maxHierarchyLevel)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override Boolean HasLink(String name1, String name2, params string[] domain)
        {
            if (base.HasLink(name1, name2, domain))
            {
                return true;
            }
            // check name1's groups
            if (domain.Length == 1)
            {
                try
                {
                    var groups = base.GetRoles(name1) ?? new List<string>();

                    foreach (var group in groups)
                    {
                        if (HasLink(group, name2, domain))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
