using System.Collections.Generic;

namespace Casbin.Model
{
    public interface IReadOnlyAssertion
    {
        public string Key { get; }

        public string Value { get; }

        public IReadOnlyDictionary<string, int> Tokens { get; }
    }
}
