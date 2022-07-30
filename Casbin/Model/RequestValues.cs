using System;
using System.Collections.Generic;

namespace Casbin.Model;

public struct RequestValues : IRequestValues
{
    public static RequestValues Empty { get; } = new();

    public string this[int index] => index switch
    {
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 0;

    internal static string ToStringValue<T>(T value)
    {
        return value as string ?? value.ToString();
    }
}

public struct RequestValues<T> : IRequestValues
{
    public RequestValues(T value) => Value1 = value;

    public T Value1 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 1;
}

public struct RequestValues<T1, T2> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2)
    {
        Value1 = value1;
        Value2 = value2;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 2;
}

public struct RequestValues<T1, T2, T3> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 3;
}

public struct RequestValues<T1, T2, T3, T4> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 4;
}

public struct RequestValues<T1, T2, T3, T4, T5> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 5;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 6;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 7;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 8;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 9;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 10;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }
    public T11 Value11 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        10 => RequestValues.ToStringValue(Value11),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 11;
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
        Value12 = value12;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }
    public T11 Value11 { get; set; }
    public T12 Value12 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        10 => RequestValues.ToStringValue(Value11),
        11 => RequestValues.ToStringValue(Value12),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 12;
}

public readonly struct ObjectListRequestValues : IRequestValues
{
    private readonly IReadOnlyList<object> _values;

    public ObjectListRequestValues(IReadOnlyList<object> values)
    {
        _values = values;
    }

    public string this[int index] => RequestValues.ToStringValue(_values[index]);
    public int Count => _values.Count;
}
