using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Persist;
using Casbin.Rbac;
#if !NET45
using Microsoft.Extensions.Logging;
#endif
namespace Casbin
{
    /// <summary>
    /// IEnforcer is the API interface of Enforcer
    /// </summary>
    public interface IEnforcer
    {
        #region Options
        public bool Enabled { get; set; }
        public bool EnabledCache { get; set; }
        public bool AutoSave { get; set; }
        public bool AutoBuildRoleLinks { get; set; }
        public bool AutoNotifyWatcher { get; set; }
        public bool AutoCleanEnforceCache { get; set; }
        #endregion

        #region Extensions
        public IEffector Effector { get; set; }
        public IModel Model { get; set; }
        public IPolicyManager PolicyManager { get; set; }
        public IAdapter Adapter { get; set; }
        public IWatcher Watcher { get; set; }
        public IRoleManager RoleManager { get; set; }
        public IEnforceCache EnforceCache { get; set; }
        public IExpressionHandler ExpressionHandler { get; set; }
#if !NET45
        public ILogger Logger { get; set; }
#endif
        #endregion

        public bool IsSynchronized { get; }
        public string ModelPath { get; }
        public bool IsFiltered { get; }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(params object[] requestValues);

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public Task<bool> EnforceAsync(params object[] requestValues);

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceEx(params object[] requestValues);
#else
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceEx(params object[] requestValues);
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExAsync(params object[] requestValues);
#else
        public Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExAsync(params object[] requestValues);
#endif

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool EnforceWithMatcher(string matcher, params object[] requestValues);

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public Task<bool> EnforceWithMatcherAsync(string matcher, params object[] requestValues);

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceExWithMatcher(string matcher, params object[] requestValues);
#else
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceExWithMatcher(string matcher,params object[] requestValues);
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="matcher">The custom matcher.</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExWithMatcherAsync(string matcher, params object[] requestValues);
#else
        public Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExWithMatcherAsync(string matcher,params object[] requestValues);
#endif
    }
}
