using System;
using System.Collections.Generic;

namespace Casbin.Rbac
{
    public interface IRoleManager
    {
        /// <summary>
        /// Supports use pattern in g.
        /// </summary>
        public Func<string, string, bool> MatchingFunc { get; set; }

        /// <summary>
        /// Supports use domain pattern in g.
        /// </summary>
        public Func<string, string, bool> DomainMatchingFunc { get; set; }

        /// <summary>
        /// Whether MatchingFunc is set.
        /// </summary>
        public bool HasPattern { get; }

        /// <summary>
        /// Whether DomainMatchingFunc is set.
        /// </summary>
        public bool HasDomainPattern { get; }

        /// <summary>
        /// Gets the roles that a user inherits. domain is a prefix to the roles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetRoles(string name, params string[] domain);

        /// <summary>
        /// Gets the users that inherits a role.
        /// domain is a prefix to the users (can be used for other purposes).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public List<string> GetUsers(string name, params string[] domain);

        /// <summary>
        /// Determines whether a link exists between two roles. role: name1
        /// inherits role: name2. domain is a prefix to the roles.
        /// </summary>
        /// <param name="name1">The first role (or a user).</param>
        /// <param name="name2">The second role.</param>
        /// <param name="domain">The domain the roles belong to.</param>
        /// <returns>Whether name1 inherits name2 (name1 has role name2).</returns>
        public bool HasLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// Adds the inheritance link between two roles. role: name1 and role:
        /// name2. domain is a prefix to the roles.
        /// </summary>
        /// <param name="name1">The first role (or user).</param>
        /// <param name="name2">The second role.</param>
        /// <param name="domain">The domain the roles belong to.</param>
        public void AddLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// Deletes the inheritance link between two roles. role: name1 and
        /// role: name2. domain is a prefix to the roles.
        /// </summary>
        /// <param name="name1">The first role (or user).</param>
        /// <param name="name2">The second role.</param>
        /// <param name="domain">The domain the roles belong to.</param>
        public void DeleteLink(string name1, string name2, params string[] domain);

        /// <summary>
        /// Clears all stored data and resets the role manager to the initial state.
        /// </summary>
        public void Clear();
    }
}
