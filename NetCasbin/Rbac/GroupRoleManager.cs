using System;
using System.Collections.Generic;

namespace NetCasbin.Rbac
{
    /// <summary>
    /// GroupRoleManager is used for authorization if the user's group is the role who has permission,
    /// but the group information is in the default format(policy start with "g") and the role information
    /// is in named format(policy start with "g2", "g3", ...).
    /// e.g.
    /// p, admin, domain1, data1, read
    /// g, alice, group1
    /// g2, group1, admin, domain1
    /// As for the previous example, alice should have the permission to read data1, but if we use the
    /// DefaultRoleManager, it will return false.
    /// GroupRoleManager is to handle this situation.
    /// </summary>
    /// 
    public class GroupRoleManager : DefaultRoleManager
    {
        /// <summary>
        /// GroupRoleManager is the constructor for creating an instance of the 
        /// GroupRoleManager implementation.
        /// </summary>
        /// <param name="maxHierarchyLevel">The maximized allowed RBAC hierarchy level.</param>
        public GroupRoleManager(int maxHierarchyLevel) : base(maxHierarchyLevel)
        {
        }

        /// <summary>
        /// Determines whether role: name1 inherits role: name2. 
        /// domain is a prefix to the roles.
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override bool HasLink(string name1, string name2, params string[] domain)
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

                    foreach (string group in groups)
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
