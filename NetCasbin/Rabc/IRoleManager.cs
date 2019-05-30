using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin.Rabc
{
    public interface IRoleManager
    {
        /// <summary>
        /// 清除所有的数据并将角色管理器重置为初始状态
        /// </summary>
        void Clear();

        /// <summary>
        /// 新增两个角色之间的继承关系
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="domain"></param>
        void AddLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// 删除两个角色之间的继承关系
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="domain"></param>
        void DeleteLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// 两个角色之间是否有链接（第一个角色是否继承第二个角色）
        /// </summary>
        /// <param name="name1">第一个角色（或者用户）</param>
        /// <param name="name2">第二个角色</param>
        /// <param name="domain">角色所属的域</param>
        /// <returns>角色1是否继承角色2（角色1具有角色2）</returns>
        Boolean HasLink(String name1, String name2, params string[] domain);

        List<String> GetRoles(String name, params string[] domain);

        List<string> GetUsers(String name, params string[] domain);
    }
}
