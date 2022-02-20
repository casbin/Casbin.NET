using System;
using System.Collections;
using System.Collections.Generic;

namespace Casbin.Model;

public record Policy<T1>(T1 Value1) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 1;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1>>(this);
}

public record Policy<T1, T2>(T1 Value1, T2 Value2) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 2;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2>>(this);
}

public record Policy<T1, T2, T3>(T1 Value1, T2 Value2, T3 Value3) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 3;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3>>(this);
};

public record Policy<T1, T2, T3, T4>(T1 Value1, T2 Value2, T3 Value3, T4 Value4) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 4;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4>>(this);
}

public record Policy<T1, T2, T3, T4, T5>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 5;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 6;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6, T7>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        6 => Policy.ToString(Value7),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 7;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6, T7, T8>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        6 => Policy.ToString(Value7),
        7 => Policy.ToString(Value8),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 8;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        6 => Policy.ToString(Value7),
        7 => Policy.ToString(Value8),
        8 => Policy.ToString(Value9),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 9;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        6 => Policy.ToString(Value7),
        7 => Policy.ToString(Value8),
        8 => Policy.ToString(Value9),
        9 => Policy.ToString(Value10),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 10;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        6 => Policy.ToString(Value7),
        7 => Policy.ToString(Value8),
        8 => Policy.ToString(Value9),
        9 => Policy.ToString(Value10),
        10 => Policy.ToString(Value11),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 11;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(this);
}

public record Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12) : IPolicyValues
{
    public string this[int index] => index switch
    {
        0 => Policy.ToString(Value1),
        1 => Policy.ToString(Value2),
        2 => Policy.ToString(Value3),
        3 => Policy.ToString(Value4),
        4 => Policy.ToString(Value5),
        5 => Policy.ToString(Value6),
        6 => Policy.ToString(Value7),
        7 => Policy.ToString(Value8),
        8 => Policy.ToString(Value9),
        9 => Policy.ToString(Value10),
        10 => Policy.ToString(Value11),
        11 => Policy.ToString(Value12),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 12;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(this);
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

    public void Dispose() { }
    public bool MoveNext() => ++_index < _value.Count;
    public void Reset() => _index = -1;
}

internal class PolicyValues : IPolicyValues
{
    private readonly IReadOnlyList<string> _values;

    public PolicyValues(IReadOnlyList<string> values)
    {
        _values = values;
    }

    public string this[int index] => _values[index];

    public int Count => _values.Count;

    public IEnumerator<string> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
}

public class Policy : IPolicyValues
{
    public string this[int index] => index switch
    {
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 0;

    public IEnumerator<string> GetEnumerator() => new PolicyEnumerator<Policy>(this);
    IEnumerator IEnumerable.GetEnumerator() => new PolicyEnumerator<Policy>(this);

    public static Policy Empty { get; } = new();

    internal static string ToString<T>(T value)
    {
        return value as string ?? value.ToString();
    }

    public static bool SupportGeneric(int count)
    {
        return count is >= 1 and <= 12;
    }

    public static Policy<T1> Create<T1>(T1 value1)
    {
        return new Policy<T1>(value1);
    }

    public static Policy<T1, T2> Create<T1, T2>(T1 value1, T2 value2)
    {
        return new Policy<T1, T2>(value1, value2);
    }

    public static Policy<T1, T2, T3> Create<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
    {
        return new Policy<T1, T2, T3>(value1, value2, value3);
    }

    public static Policy<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3,
        T4 value4)
    {
        return new Policy<T1, T2, T3, T4>(value1, value2, value3, value4);
    }

    public static Policy<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5)
    {
        return new Policy<T1, T2, T3, T4, T5>(value1, value2, value3, value4, value5);
    }

    public static Policy<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6)
    {
        return new Policy<T1, T2, T3, T4, T5, T6>(value1, value2, value3, value4, value5, value6);
    }

    public static Policy<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        return new Policy<T1, T2, T3, T4, T5, T6, T7>(value1, value2, value3, value4, value5, value6, value7);
    }

    public static Policy<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        return new Policy<T1, T2, T3, T4, T5, T6, T7, T8>(value1, value2, value3, value4, value5, value6, value7, value8);
    }

    public static Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        return new Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value1, value2, value3, value4, value5, value6, value7, value8, value9);
    }

    public static Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        return new Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10);
    }

    public static Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
    {
        return new Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11);
    }

    public static Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12)
    {
        return new Policy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12);
    }

    public static IPolicyValues CreateOnlyString(IReadOnlyList<string> values)
    {
        return values.Count switch
        {
            1 => Create(values[0]),
            2 => Create(values[0], values[1]),
            3 => Create(values[0], values[1], values[2]),
            4 => Create(values[0], values[1], values[2], values[3]),
            5 => Create(values[0], values[1], values[2], values[3],
                values[4]),
            6 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5]),
            7 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6]),
            8 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7]),
            9 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8]),
            10 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8],
                values[9]),
            11 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8],
                values[9], values[10]),
            12 => Create(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8],
                values[9], values[10], values[11]),
            _ => new PolicyValues(values)
        };
    }
}
