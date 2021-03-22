using System;

namespace NetCasbin.Caching
{
    public class ReaderWriterEnforceCacheOptions
    {
        public TimeSpan WaitTimeOut { get; set; } = TimeSpan.FromMilliseconds(50);
    }
}
