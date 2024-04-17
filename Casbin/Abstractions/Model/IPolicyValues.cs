using System.Collections.Generic;

namespace Casbin.Model;

public interface IPolicyValues : IReadOnlyList<string>
{
    public new string this[int index] { get; }

    public string ToText();

    public bool Equals(IPolicyValues other);
}
