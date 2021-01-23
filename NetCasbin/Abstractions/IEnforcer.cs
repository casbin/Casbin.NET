using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCasbin.Abstractions
{
    /// <summary>
    /// IEnforcer is the API interface of Enforcer
    /// </summary>
    [Obsolete("The interface will be changed a lot at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/56 to know more information.")]
    public interface IEnforcer : IManagementEnforcer
    {
        /// <summary>
        /// Gets the roles that a user has.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<string> GetRolesForUser(string name, string domain = null);


        /// <summary>
        /// Gets the users that has a role.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<string> GetUsersForRole(string name, string domain = null);

        /// <summary>
        /// Gets the users that has roles.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        List<string> GetUsersForRoles(string[] names);

        /// <summary>
        /// Determines whether a user has a role.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        bool HasRoleForUser(string name, string role, string domain = null);

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        bool AddRoleForUser(string user, string role, string domain = null);

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        Task<bool> AddRoleForUserAsync(string user, string role, string domain = null);

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have the role (aka not affected).</returns>
        bool DeleteRoleForUser(string user, string role, string domain = null);

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have the role (aka not affected).</returns>
        Task<bool> DeleteRoleForUserAsync(string user, string role, string domain = null);

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        bool DeleteRolesForUser(string user, string domain = null);

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        Task<bool> DeleteRolesForUserAsync(string user, string domain = null);

        /// <summary>
        /// DeleteUser deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        bool DeleteUser(string user);

        /// <summary>
        /// DeleteUser deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        Task<bool> DeleteUserAsync(string user);

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role"></param>
        bool DeleteRole(string role);

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role"></param>
        Task<bool> DeleteRoleAsync(string role);

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        bool DeletePermission(List<string> permission);

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        Task<bool> DeletePermissionAsync(List<string> permission);

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        bool DeletePermission(params string[] permission);

        /// <summary>
        /// DeletePermission deletes a permission. 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        Task<bool> DeletePermissionAsync(params string[] permission);

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        bool AddPermissionForUser(string user, List<string> permission);

        /// <summary>
        /// Adds multiple permissions for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        Task<bool> AddPermissionForUserAsync(string user, List<string> permission);

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        bool AddPermissionForUser(string user, params string[] permission);

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        Task<bool> AddPermissionForUserAsync(string user, params string[] permission);

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        bool DeletePermissionForUser(string user, List<string> permission);

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        Task<bool> DeletePermissionForUserAsync(string user, List<string> permission);

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool DeletePermissionForUser(string user, params string[] permission);

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> DeletePermissionForUserAsync(string user, params string[] permission);

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role. 
        /// </summary>
        /// <param name="user">User or role</param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        bool DeletePermissionsForUser(string user);

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role. 
        /// </summary>
        /// <param name="user">User or role</param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        Task<bool> DeletePermissionsForUserAsync(string user);

        /// <summary>
        /// Gets permissions for a user or role.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<List<string>> GetPermissionsForUser(string user, string domain = null);

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasPermissionForUser(string user, params string[] permission);

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasPermissionForUser(string user, List<string> permission);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<string> GetRolesForUserInDomain(string name, string domain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<List<string>> GetPermissionsForUserInDomain(string user, string domain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        bool AddRoleForUserInDomain(string user, string role, string domain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task<bool> AddRoleForUserInDomainAsync(string user, string role, string domain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        bool DeleteRoleForUserInDomain(string user, string role, string domain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task<bool> DeleteRoleForUserInDomainAsync(string user, string role, string domain);

        /// <summary>
        /// Gets implicit roles that a user has.
        /// Compared to GetRolesForUser(), this function retrieves indirect roles besides direct roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<string> GetImplicitRolesForUser(string name, string domain = null);

        /// <summary>
        /// <para>Gets implicit permissions for a user or role.</para>
        /// <para>Compared to GetPermissionsForUser(), this function retrieves permissions for inherited roles.</para> 
        /// <para>For example:</para>
        /// <para>p, admin, data1, read</para>
        /// <para>p, alice, data2, read</para>
        /// <para>g, alice, admin </para>
        /// <para>GetPermissionsForUser("alice") can only get: [["alice", "data2", "read"]].</para>
        /// <para>But GetImplicitPermissionsForUser("alice") will get: [["admin", "data1", "read"], ["alice", "data2", "read"]].</para>
        /// </summary>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<List<string>> GetImplicitPermissionsForUser(string user,  string domain = null);

    }

}
