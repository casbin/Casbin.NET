using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin.Rabc
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
                    var groups = base.GetRoles(name1);
                    if (groups == null)
                    {
                        groups = new List<string>();
                    }

                    foreach (String group in groups)
                    {
                        if (HasLink(group, name2, domain))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
