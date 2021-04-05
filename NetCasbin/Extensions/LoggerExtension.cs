using System.Collections.Generic;
using System.Linq;
#if !NET45
using Microsoft.Extensions.Logging;

namespace NetCasbin.Extensions
{
    public static class LoggerExtension
    {
        public static void LogEnforceCachedResult<T1, T2, T3>(this ILogger logger, T1 value1, T2 value2, T3 value3, bool result)
        {
            logger.LogInformation("Request: {1}, {2}, {3} ---> {0} (cached)", result, value1, value2, value3);
        }

        public static void LogEnforceResult<T1, T2, T3>(this ILogger logger, T1 value1, T2 value2, T3 value3, bool result)
        {
            logger.LogInformation("Request: {1}, {2}, {3} ---> {0}", result, 
                string.Join(", ", value1, value2, value3));
        }

        public static void LogEnforceResult<T1, T2, T3>(this ILogger logger, T1 value1, T2 value2, T3 value3,
            bool result, IEnumerable<IEnumerable<string>> explains)
        {
            logger.LogInformation("Request: {1}, {2}, {3} ---> {0}\nHit Policy: {4}", result, 
                value1, value2, value3,
                string.Join("\n", explains.Select(explain =>
                    string.Join(", ", explain))));
        }

        public static void LogEnforceCachedResult(this ILogger logger, IEnumerable<object> requestValues, bool result)
        {
            logger.LogInformation("Request: {1} ---> {0} (cached)", result, 
                string.Join(", ", requestValues));
        }

        public static void LogEnforceResult(this ILogger logger, IEnumerable<object> requestValues, bool result)
        {
            logger.LogInformation("Request: {1} ---> {0}", result, 
                string.Join(", ", requestValues));
        }

        public static void LogEnforceResult(this ILogger logger, IEnumerable<object> requestValues,
            bool result, IEnumerable<IEnumerable<string>> explains)
        {
            logger.LogInformation("Request: {1} ---> {0}\nHit Policy: {2}", result, 
                string.Join(", ", requestValues),
                string.Join("\n", explains.Select(explain =>
                    string.Join(", ", explain))));
        }
    }
}
#endif
