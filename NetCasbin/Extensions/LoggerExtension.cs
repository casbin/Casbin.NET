#if !NET452
using System.Collections.Generic;
using System.Linq;
using Casbin.Model;
using Microsoft.Extensions.Logging;

namespace Casbin
{
    public static class LoggerExtension
    {
        public static void LogEnforceCachedResult<TRequest>(this ILogger logger, in TRequest requestValues, bool result)
            where TRequest : IRequestValues
        {
            logger.LogInformation("Request: {1} ---> {0} (cached)", result, requestValues);
        }

        public static void LogEnforceResult<TRequest>(this ILogger logger, in TRequest requestValues, bool result)
            where TRequest : IRequestValues
        {
            logger.LogInformation("Request: {1} ---> {0}", result, requestValues);
        }

        public static void LogEnforceResult<TRequest>(this ILogger logger, in TRequest requestValues,
            bool result, IEnumerable<IEnumerable<string>> explains)
            where TRequest : IRequestValues
        {
            logger.LogInformation("Request: {1} ---> {0}\nHit Store: {2}", result, requestValues,
                string.Join("\n", explains.Select(explain =>
                    string.Join(", ", explain))));
        }
    }
}
#endif
