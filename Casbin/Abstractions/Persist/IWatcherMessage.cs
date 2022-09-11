using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IWatcherMessage
    {
        public PolicyOperation Operation { get; }
        public string Section { get; }
        public string PolicyType { get; }
        public int FieldIndex { get; }
        public IPolicyValues Values { get; }
        public IPolicyValues NewValues { get; }
        public IReadOnlyList<IPolicyValues> ValuesList { get; }
        public IReadOnlyList<IPolicyValues> NewValuesList { get; }
    }
}


