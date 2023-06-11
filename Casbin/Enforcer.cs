using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Persist;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace Casbin
{
    public partial class Enforcer : IEnforcer
    {
        public Enforcer()
        {
        }

        public Enforcer(string modelPath, string policyPath, EnforcerOptions options = default)
            : this(modelPath, new FileAdapter(policyPath), options)
        {
        }

        public Enforcer(string modelPath, IReadOnlyAdapter adapter = default, EnforcerOptions options = default)
            : this(DefaultModel.CreateFromFile(modelPath), adapter, options)
        {
        }

        public Enforcer(IModel model, IReadOnlyAdapter adapter = default, EnforcerOptions options = default)
        {
            options ??= new EnforcerOptions();

            Enabled = options.Enabled;
            EnabledCache = options.EnabledCache;
            AutoNotifyWatcher = options.AutoNotifyWatcher;
            AutoBuildRoleLinks = options.AutoBuildRoleLinks;
            AutoCleanEnforceCache = options.AutoCleanEnforceCache;

            this.SetModel(model);
            if (adapter is not null)
            {
                this.SetAdapter(adapter);
            }

            if (options.AutoLoadPolicy)
            {
                if (Adapter is IFilteredAdapter && options.AutoLoadPolicyFilter is not null)
                {
                    this.LoadFilteredPolicy(options.AutoLoadPolicyFilter);
                }
                else
                {
                    this.LoadPolicy();
                }
            }

            model.SortPolicy();
        }

        #region Options

        public bool Enabled { get; set; }
        public bool EnabledCache { get; set; }
        public bool AutoBuildRoleLinks { get; set; }
        public bool AutoNotifyWatcher { get; set; }
        public bool AutoCleanEnforceCache { get; set; }

        #endregion

        #region Extensions

        public IEffector Effector
        {
            get => Model.EffectorHolder.Effector;
            set => Model.EffectorHolder.Effector = value;
        }

        public IReadOnlyWatcher Watcher
        {
            get => Model.WatcherHolder.Watcher;
            set => Model.WatcherHolder.Watcher = value;
        }

        public IModel Model { get; set; }

        public IReadOnlyAdapter Adapter
        {
            get => Model.AdapterHolder.Adapter;
            set => Model.AdapterHolder.Adapter = value;
        }

        public IEnforceCache EnforceCache
        {
            get => Model.EnforceCache;
            set => Model.EnforceCache = value;
        }

#if !NET452
        public ILogger Logger { get; set; }
#endif

        #endregion

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce<TRequest>(EnforceContext context, TRequest requestValues) where TRequest : IRequestValues
        {
            if (context.HandleOptionAndCached)
            {
                return InternalEnforce(in context, in requestValues);
            }

            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache)
            {
                if (EnforceCache.TryGetResult(requestValues, out bool cachedResult))
                {
#if !NET452
                    this.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                    return cachedResult;
                }
            }

            bool result = InternalEnforce(in context, in requestValues);

            if (EnabledCache)
            {
                EnforceCache.TrySetResult(requestValues, result);
            }
#if !NET452
            this.LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceAsync<TRequest>(EnforceContext context, TRequest requestValues)
            where TRequest : IRequestValues
        {
            if (context.HandleOptionAndCached)
            {
                return await InternalEnforceAsync(context, requestValues);
            }

            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache)
            {
                bool? cachedResult = await EnforceCache.TryGetResultAsync(requestValues);
                if (cachedResult.HasValue)
                {
#if !NET452
                    this.LogEnforceCachedResult(requestValues, cachedResult.Value);
#endif
                    return cachedResult.Value;
                }
            }

            context.HandleOptionAndCached = true;
            bool result = await InternalEnforceAsync(context, requestValues);

            if (EnabledCache)
            {
                await EnforceCache.TrySetResultAsync(requestValues, result);
            }
#if !NET452
            this.LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        public IEnumerable<bool> BatchEnforce<TRequest>(EnforceContext context,
            IEnumerable<TRequest> requestValues) where TRequest : IRequestValues
        {
            foreach (TRequest requestValue in requestValues)
            {
                yield return Enforce(context, requestValue);
            }
        }

        public IEnumerable<bool> ParallelBatchEnforce<TRequest>(EnforceContext context,
            IReadOnlyList<TRequest> requestValues, int maxDegreeOfParallelism = -1) where TRequest : IRequestValues
        {
            int valuesCount = requestValues.Count;
            if (valuesCount is 0)
            {
                return new bool[0];
            }

            bool[] results = new bool[valuesCount];
            Parallel.For(0, valuesCount, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                index => { results[index] = Enforce(context, requestValues[index]); });
            return results;
        }

#if !NET452
        public async IAsyncEnumerable<bool> BatchEnforceAsync<TRequest>(EnforceContext context,
            IEnumerable<TRequest> requestValues)
            where TRequest : IRequestValues
        {
            foreach (var requestValue in requestValues)
            {
                bool result = await EnforceAsync(context, requestValue);
                yield return result;
            }
        }
#else
        public async Task<IEnumerable<bool>> BatchEnforceAsync<TRequest>(EnforceContext context,
            IEnumerable<TRequest> requestValues)
            where TRequest : IRequestValues
        {
            List<bool> results = new();
            foreach (TRequest requestValue in requestValues)
            {
                results.Add(await EnforceAsync(context, requestValue));
            }
            return results;
        }
#endif
    }
}
