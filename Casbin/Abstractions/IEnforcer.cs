using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Persist;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace Casbin
{
#if !NET452
    using BatchEnforceAsyncResults = IAsyncEnumerable<bool>;

#else
    using BatchEnforceAsyncResults = Task<IEnumerable<bool>>;
#endif

    /// <summary>
    /// IEnforcer is the API interface of Enforcer
    /// </summary>
    public interface IEnforcer
    {
        public class EnforcerOptions
        {
            public bool Enabled { get; set; } = true;
            public bool EnabledCache { get; set; } = true;

            public bool AutoBuildRoleLinks { get; set; } = true;
            public bool AutoNotifyWatcher { get; set; } = true;
            public bool AutoCleanEnforceCache { get; set; } = true;
            public bool AutoLoadPolicy { get; set; } = true;
            public Filter AutoLoadPolicyFilter { get; set; } = null;
        }

        /// <summary>
        ///     Decides whether a "subject" can access a "object" with the operation
        ///     "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestValues">
        ///     The request needs to be mediated, usually an array of strings,
        ///     can be class instances if ABAC is used.
        /// </param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce<TRequest>(EnforceContext context, TRequest requestValues) where TRequest : IRequestValues;

        /// <summary>
        ///     Decides whether a "subject" can access a "object" with the operation
        ///     "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestValues">
        ///     The request needs to be mediated, usually an array of strings,
        ///     can be class instances if ABAC is used.
        /// </param>
        /// <returns>Whether to allow the request.</returns>
        public Task<bool> EnforceAsync<TRequest>(EnforceContext context, TRequest requestValues)
            where TRequest : IRequestValues;

        /// <summary>
        ///     Decides whether some "subject" can access corresponding "object" with the operation
        ///     "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">
        ///     The requests needs to be mediated, whose element is usually an array of strings
        ///     but can be class instances if ABAC is used.
        /// </param>
        /// <returns>Whether to allow the requests.</returns>
        public IEnumerable<bool> BatchEnforce<TRequest>(EnforceContext context, IEnumerable<TRequest> requestValues)
            where TRequest : IRequestValues;

        /// <summary>
        ///     Decides whether some "subject" can access corresponding "object" with the operation
        ///     "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">
        ///     The requests needs to be mediated, whose element is usually an array of strings
        ///     but can be class instances if ABAC is used.
        /// </param>
        /// <returns>Whether to allow the requests.</returns>
        public BatchEnforceAsyncResults BatchEnforceAsync<TRequest>(EnforceContext context,
            IEnumerable<TRequest> requestValues)
            where TRequest : IRequestValues;

        #region Options

        public EnforcerOptions Options { get; set; }
        public bool Enabled { get; set; }
        public bool EnabledCache { get; set; }
        public bool AutoBuildRoleLinks { get; set; }
        public bool AutoNotifyWatcher { get; set; }
        public bool AutoCleanEnforceCache { get; set; }

        #endregion

        #region Extensions

        public IEffector Effector { get; set; }
        public IModel Model { get; set; }
        public IReadOnlyAdapter Adapter { get; set; }
        public IReadOnlyWatcher Watcher { get; set; }
        public IEnforceCache EnforceCache { get; set; }
#if !NET452
        public ILogger Logger { get; set; }
#endif

        #endregion
    }
}
