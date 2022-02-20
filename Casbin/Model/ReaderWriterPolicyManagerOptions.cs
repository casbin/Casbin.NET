using System;

namespace Casbin.Model
{
    public class ReaderWriterPolicyManagerOptions
    {
        public TimeSpan WaitTimeOut { get; } = TimeSpan.FromMilliseconds(50);
    }
}
