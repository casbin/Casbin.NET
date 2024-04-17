using System;
using System.Collections;
using System.Collections.Generic;

namespace Casbin.Model;

public record PolicyValues : IPolicyValues
{
    protected string Text;

    private List<string> Values{ get; set; } = new();

    public static PolicyValues Empty { get; } = new();

    public int Count;

    public bool IsReadOnly => false;

    int ICollection<string>.Count => Values.Count;

    string IList<string>.this[int index] { get => Values[index]; set => new NotImplementedException(); }

    public string this[int index] => Values[index];

    public virtual IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues>(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public string ToText() => Text ??= ToText(this);

    public bool Equals(IPolicyValues other) => Equals(this, other);

    internal static string ToText(IEnumerable<string> values)
        => string.Join(PermConstants.PolicySeparatorString, values);

    internal static bool Equals<T>(T values, T other) where T : IPolicyValues
        => values.Count == other.Count && values.ToText().Equals(other.ToText());

    internal static string ToStringValue<T>(T value) => value as string ?? value.ToString();

    public int IndexOf(string item) => ((IList<string>)Empty).IndexOf(item);

    public void Insert(int index, string item) => ((IList<string>)Empty).Insert(index, item);

    public void RemoveAt(int index) => ((IList<string>)Empty).RemoveAt(index);

    public void Add(string item) => Values.Add(item);

    public void Clear() => Values.Clear();

    public bool Contains(string item) => Values.Contains(item);

    public void CopyTo(string[] array, int arrayIndex) => Values.CopyTo(array, arrayIndex);

    public bool Remove(string item) => Values.Remove(item);

    public PolicyValues(params object[] values)
    {
        foreach (var item in values)
        {
            Values.Add(item.ToString());
        }
        Text = ToText(Values);
    }

    public PolicyValues(string text, IList<string> values)
    {
        Text = text;
        foreach (var item in values)
        {
            Values.Add(item.ToString());
        }
        Count = Values.Count;
    }

    public PolicyValues(string text, params object[] values)
    {
        Text = text;
        foreach (var item in values)
        {
            Values.Add(item.ToString());
        }
        Count = Values.Count;
    }
}

internal record DummyPolicyValues(string Value1 = "", string Value2 = "", string Value3 = "",
    string Value4 = "", string Value5 = "", string Value6 = "",
    string Value7 = "", string Value8 = "", string Value9 = "",
    string Value10 = "", string Value11 = "", string Value12 = "") : PolicyValues
{
    public static new readonly DummyPolicyValues Empty = new();
    public new int Count => 12;

    public new string this[int index] => index switch
    {
        0 => Value1,
        1 => Value2,
        2 => Value3,
        3 => Value4,
        4 => Value5,
        5 => Value6,
        6 => Value7,
        7 => Value8,
        8 => Value9,
        9 => Value10,
        10 => Value11,
        11 => Value12,
        _ => ""
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<DummyPolicyValues>(this);
}

internal record StringListPolicyValues : PolicyValues
{
    private readonly IList<string> _values;

    public StringListPolicyValues(IList<string> values) => _values = values;

    public new int Count => _values.Count;

    public new string this[int index] => _values[index];

    public override IEnumerator<string> GetEnumerator() => _values.GetEnumerator();
}

    internal struct PolicyEnumerator<T> : IEnumerator<string> where T : IPolicyValues
{
    public PolicyEnumerator(T value)
    {
        _value = value;
        _index = -1;
    }

    private int _index;
    private readonly T _value;

    public object Current => _value[_index];
    string IEnumerator<string>.Current => _value[_index];

    public void Dispose()
    {
    }

    public bool MoveNext() => ++_index < _value.Count;
    public void Reset() => _index = -1;
}


