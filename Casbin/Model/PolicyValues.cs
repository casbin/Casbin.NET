using System;
using System.Collections;
using System.Collections.Generic;

namespace Casbin.Model;

public record PolicyValues : IPolicyValues
{
    protected string Text;

    public static PolicyValues Empty { get; } = new();
    public virtual int Count => 0;
    public virtual string this[int index] => throw new ArgumentOutOfRangeException(nameof(index));
    public virtual IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues>(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public string ToText() => Text ??= ToText(this);
    public bool Equals(IPolicyValues other) => Equals(this, other);

    internal static string ToText<T>(T values) where T : IPolicyValues
        => string.Join(PermConstants.PolicySeparatorString, values);

    internal static string ToText(IEnumerable<string> values)
        => string.Join(PermConstants.PolicySeparatorString, values);

    internal static bool Equals<T>(T values, T other) where T : IPolicyValues
        => values.Count == other.Count && values.ToText().Equals(other.ToText());

    internal static string ToStringValue<T>(T value) => value as string ?? value.ToString();
}

public record PolicyValues<T1>(T1 Value1) : PolicyValues
{
    private string _stringValue1;

    internal PolicyValues(string text, T1 value1, string stringValue1) : this(value1)
    {
        _stringValue1 = stringValue1;
        Text = text;
    }

    public override int Count => 1;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues<T1>>(this);
}

public record PolicyValues<T1, T2>(T1 Value1, T2 Value2) : PolicyValues
{
    private string _stringValue1, _stringValue2;

    internal PolicyValues(string text, T1 value1, string stringValue1, T2 value2, string stringValue2) : this(value1,
        value2)
    {
        _stringValue1 = stringValue1;
        _stringValue2 = stringValue2;
        Text = text;
    }

    public override int Count => 2;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        1 => _stringValue2 ??= ToStringValue(Value2),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues<T1, T2>>(this);
}

public record PolicyValues<T1, T2, T3>(T1 Value1, T2 Value2, T3 Value3) : PolicyValues
{
    private string _stringValue1, _stringValue2, _stringValue3;

    internal PolicyValues(string text, T1 value1, string stringValue1, T2 value2, string stringValue2,
        T3 value3, string stringValue3) : this(value1, value2, value3)
    {
        _stringValue1 = stringValue1;
        _stringValue2 = stringValue2;
        _stringValue3 = stringValue3;
        Text = text;
    }

    public override int Count => 3;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        1 => _stringValue2 ??= ToStringValue(Value2),
        2 => _stringValue3 ??= ToStringValue(Value3),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues<T1, T2, T3>>(this);
}

public record PolicyValues<T1, T2, T3, T4>(T1 Value1, T2 Value2, T3 Value3, T4 Value4) : PolicyValues
{
    private string _stringValue1, _stringValue2, _stringValue3, _stringValue4;

    internal PolicyValues(string text, T1 value1, string stringValue1, T2 value2, string stringValue2,
        T3 value3, string stringValue3, T4 value4, string stringValue4) : this(value1, value2, value3, value4)
    {
        _stringValue1 = stringValue1;
        _stringValue2 = stringValue2;
        _stringValue3 = stringValue3;
        _stringValue4 = stringValue4;
        Text = text;
    }

    public override int Count => 4;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        1 => _stringValue2 ??= ToStringValue(Value2),
        2 => _stringValue3 ??= ToStringValue(Value3),
        3 => _stringValue4 ??= ToStringValue(Value4),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues<T1, T2, T3, T4>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5) : PolicyValues
{
    private string _stringValue1, _stringValue2, _stringValue3, _stringValue4, _stringValue5;

    internal PolicyValues(string text, T1 value1, string stringValue1, T2 value2, string stringValue2,
        T3 value3, string stringValue3, T4 value4, string stringValue4, T5 value5, string stringValue5)
        : this(value1, value2, value3, value4, value5)
    {
        _stringValue1 = stringValue1;
        _stringValue2 = stringValue2;
        _stringValue3 = stringValue3;
        _stringValue4 = stringValue4;
        _stringValue5 = stringValue5;
        Text = text;
    }

    public override int Count => 5;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        1 => _stringValue2 ??= ToStringValue(Value2),
        2 => _stringValue3 ??= ToStringValue(Value3),
        3 => _stringValue4 ??= ToStringValue(Value4),
        4 => _stringValue5 ??= ToStringValue(Value5),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() => new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6>
    (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6) : PolicyValues
{
    private string _stringValue1, _stringValue2, _stringValue3, _stringValue4, _stringValue5, _stringValue6;

    internal PolicyValues(string text, T1 value1, string stringValue1, T2 value2, string stringValue2,
        T3 value3, string stringValue3, T4 value4, string stringValue4, T5 value5, string stringValue5,
        T6 value6, string stringValue6) : this(value1, value2, value3, value4, value5, value6)
    {
        _stringValue1 = stringValue1;
        _stringValue2 = stringValue2;
        _stringValue3 = stringValue3;
        _stringValue4 = stringValue4;
        _stringValue5 = stringValue5;
        _stringValue6 = stringValue6;
        Text = text;
    }

    public override int Count => 6;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        1 => _stringValue2 ??= ToStringValue(Value2),
        2 => _stringValue3 ??= ToStringValue(Value3),
        3 => _stringValue4 ??= ToStringValue(Value4),
        4 => _stringValue5 ??= ToStringValue(Value5),
        5 => _stringValue6 ??= ToStringValue(Value6),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6, T7>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6,
    T7 Value7) : PolicyValues
{
    private string _stringValue1,
        _stringValue2,
        _stringValue3,
        _stringValue4,
        _stringValue5,
        _stringValue6,
        _stringValue7;

    internal PolicyValues(string text, T1 value1, string stringValue1, T2 value2, string stringValue2,
        T3 value3, string stringValue3, T4 value4, string stringValue4, T5 value5, string stringValue5,
        T6 value6, string stringValue6, T7 value7, string stringValue7) : this(value1, value2, value3, value4, value5,
        value6, value7)
    {
        _stringValue1 = stringValue1;
        _stringValue2 = stringValue2;
        _stringValue3 = stringValue3;
        _stringValue4 = stringValue4;
        _stringValue5 = stringValue5;
        _stringValue6 = stringValue6;
        _stringValue7 = stringValue7;
        Text = text;
    }

    public override int Count => 7;

    public override string this[int index] => index switch
    {
        0 => _stringValue1 ??= ToStringValue(Value1),
        1 => _stringValue2 ??= ToStringValue(Value2),
        2 => _stringValue3 ??= ToStringValue(Value3),
        3 => _stringValue4 ??= ToStringValue(Value4),
        4 => _stringValue5 ??= ToStringValue(Value5),
        5 => _stringValue6 ??= ToStringValue(Value6),
        6 => _stringValue7 ??= ToStringValue(Value7),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6, T7>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5,
    T6 Value6,
    T7 Value7, T8 Value8) : PolicyValues
{
    public override int Count => 8;

    public override string this[int index] => index switch
    {
        0 => ToStringValue(Value1),
        1 => ToStringValue(Value2),
        2 => ToStringValue(Value3),
        3 => ToStringValue(Value4),
        4 => ToStringValue(Value5),
        5 => ToStringValue(Value6),
        6 => ToStringValue(Value7),
        7 => ToStringValue(Value8),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5,
    T6 Value6, T7 Value7, T8 Value8, T9 Value9) : PolicyValues
{
    public override int Count => 9;

    public override string this[int index] => index switch
    {
        0 => ToStringValue(Value1),
        1 => ToStringValue(Value2),
        2 => ToStringValue(Value3),
        3 => ToStringValue(Value4),
        4 => ToStringValue(Value5),
        5 => ToStringValue(Value6),
        6 => ToStringValue(Value7),
        7 => ToStringValue(Value8),
        8 => ToStringValue(Value9),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 Value1, T2 Value2, T3 Value3, T4 Value4,
    T5 Value5,
    T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10) : PolicyValues
{
    public override int Count => 10;

    public override string this[int index] => index switch
    {
        0 => ToStringValue(Value1),
        1 => ToStringValue(Value2),
        2 => ToStringValue(Value3),
        3 => ToStringValue(Value4),
        4 => ToStringValue(Value5),
        5 => ToStringValue(Value6),
        6 => ToStringValue(Value7),
        7 => ToStringValue(Value8),
        8 => ToStringValue(Value9),
        9 => ToStringValue(Value10),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 Value1, T2 Value2, T3 Value3, T4 Value4,
    T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11) : PolicyValues
{
    public override int Count => 11;

    public override string this[int index] => index switch
    {
        0 => ToStringValue(Value1),
        1 => ToStringValue(Value2),
        2 => ToStringValue(Value3),
        3 => ToStringValue(Value4),
        4 => ToStringValue(Value5),
        5 => ToStringValue(Value6),
        6 => ToStringValue(Value7),
        7 => ToStringValue(Value8),
        8 => ToStringValue(Value9),
        9 => ToStringValue(Value10),
        10 => ToStringValue(Value11),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(this);
}

public record PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4,
    T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12) : PolicyValues
{
    public override int Count => 12;

    public override string this[int index] => index switch
    {
        0 => ToStringValue(Value1),
        1 => ToStringValue(Value2),
        2 => ToStringValue(Value3),
        3 => ToStringValue(Value4),
        4 => ToStringValue(Value5),
        5 => ToStringValue(Value6),
        6 => ToStringValue(Value7),
        7 => ToStringValue(Value8),
        8 => ToStringValue(Value9),
        9 => ToStringValue(Value10),
        10 => ToStringValue(Value11),
        11 => ToStringValue(Value12),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(this);
}

public record StringPolicyValues(string Value1 = "", string Value2 = "", string Value3 = "",
    string Value4 = "", string Value5 = "", string Value6 = "",
    string Value7 = "", string Value8 = "", string Value9 = "",
    string Value10 = "", string Value11 = "", string Value12 = "") : PolicyValues
{
    public static new readonly StringPolicyValues Empty = new();
    public override int Count => 12;

    public override string this[int index] => index switch
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
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public override IEnumerator<string> GetEnumerator() =>
        new PolicyEnumerator<StringPolicyValues>(this);
}

internal record StringListPolicy : PolicyValues
{
    private readonly IReadOnlyList<string> _values;

    public StringListPolicy(IReadOnlyList<string> values) => _values = values;

    public override int Count => _values.Count;
    public override string this[int index] => _values[index];
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
