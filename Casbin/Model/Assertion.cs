using System.Collections.Generic;

namespace Casbin.Model
{
    /// <summary>
    /// Represents an expression in a section of the model.
    /// For example: r = sub, obj, act
    /// </summary>
    public abstract class Assertion : IReadOnlyAssertion
    {
        public string Key { get; internal set; }
        public string Value { get; internal set; }
        public IReadOnlyDictionary<string, int> Tokens { get; internal set; }
    }
}
