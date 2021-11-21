using System;

namespace Casbin.Caching
{
    public class EnforceCacheOptions
    {
        public TimeSpan WaitTimeOut { get; set; } = TimeSpan.FromMilliseconds(50);
    }
}
