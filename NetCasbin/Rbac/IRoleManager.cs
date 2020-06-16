using System;
using System.Collections.Generic;

namespace NetCasbin.Rbac
{
    public interface IRoleManager
    {
        /// <summary>
        /// Clear clears all stored data and resets the role manager to the initial state.
        /// 清除所有的数据并将角色管理器重置为初始状态
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the inheritance link between two roles. role: name1 and role:
        /// name2. domain is a prefix to the roles.
        /// 新增两个角色之间的继承关系
        /// </summary>
        /// <param name="name1">The first role (or user).</param>
        /// <param name="name2">The second role.</param>
        /// <param name="domain">The domain the roles belong to.</param>
        void AddLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// DeleteLink deletes the inheritance link between two roles. role: name1 and
        /// role: name2. domain is a prefix to the roles.
        /// 删除两个角色之间的继承关系
        /// </summary>
        /// <param name="name1">The first role (or user).</param>
        /// <param name="name2">The second role.</param>
        /// <param name="domain">The domain the roles belong to.</param>
        void DeleteLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// Determines whether a link exists between two roles. role: name1
        /// inherits role: name2. domain is a prefix to the roles.
        /// 两个角色之间是否有链接（第一个角色是否继承第二个角色）
        /// </summary>
        /// <param name="name1">The first role (or a user).</param>
        /// <param name="name2">The second role.</param>
        /// <param name="domain">The domain the roles belong to.</param>
        /// <returns>Whether name1 inherits name2 (name1 has role name2).</returns>
        bool HasLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// Gets the roles that a user inherits. domain is a prefix to the roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<string> GetRoles(string name, params string[] domain);

        List<string> GetUsers(string name, params string[] domain);
    }
}
