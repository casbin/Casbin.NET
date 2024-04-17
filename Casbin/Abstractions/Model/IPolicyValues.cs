using System;
using System.Collections.Generic;

namespace Casbin.Model;

public interface IPolicyValues : IList<string>
{
    public new string this[int index] { get; }

    public string ToText();

    public bool Equals(IPolicyValues other);
}
