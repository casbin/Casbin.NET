using System;
using System.Collections.Generic;

namespace Casbin.Model;

public readonly record struct Request : IRequestValues
{
    public string this[int index] => index switch
    {
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 0;

    public static Request Empty { get; } = new();

    public static bool SupportGeneric(int count)
    {
        return count is >= 1 and <= 12;
    }

    internal static string ToString<T>(T value)
    {
        return value as string ?? value.ToString();
    }

    public static Request<T> Create<T>(T value)
    {
        return new Request<T>(value);
    }

    public static Request<T1, T2> Create<T1, T2>(T1 value1, T2 value2)
    {
        return new Request<T1, T2>(value1, value2);
    }

    public static Request<T1, T2, T3> Create<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
    {
        return new Request<T1, T2, T3>(value1, value2, value3);
    }

    public static Request<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3,
        T4 value4)
    {
        return new Request<T1, T2, T3, T4>(value1, value2, value3,
            value4);
    }

    public static Request<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5)
    {
        return new Request<T1, T2, T3, T4, T5>(value1, value2, value3,
            value4, value5);
    }

    public static Request<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6)
    {
        return new Request<T1, T2, T3, T4, T5, T6>(value1, value2, value3,
            value4, value5, value6);
    }

    public static Request<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7)
    {
        return new Request<T1, T2, T3, T4, T5, T6, T7>(value1, value2, value3,
            value4, value5, value6, value7);
    }

    public static Request<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        return new Request<T1, T2, T3, T4, T5, T6, T7, T8>(value1, value2, value3,
            value4, value5, value6, value7, value8);
    }

    public static Request<T1, T2, T3, T4, T5, T6, T7, T8, T9> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        return new Request<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9);
    }

    public static Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        return new Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9, value10);
    }

    public static Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
    {
        return new Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9, value10, value11);
    }

    public static Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12)
    {
        return new Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9, value10, value11, value12);
    }

    public static RequestValues Create(params object[] values)
    {
        return new RequestValues(values);
    }

    public static RequestValues Create(IReadOnlyList<object> values)
    {
        return new RequestValues(values);
    }

    public static bool TryGetStringKey<TRequest>(TRequest requestValues, out string key) where TRequest : IRequestValues
    {
        if (SupportGeneric(requestValues.Count) is false)
        {
            key = null;
            return false;
        }

        switch (requestValues.Count)
        {
            case 1:
                if (requestValues is not Request<string> value)
                {
                    key = null;
                    return false;
                }
                key = value.Value1;
                return true;
            case 2:
                if (requestValues is not Request<string, string> values2)
                {
                    key = null;
                    return false;
                }
                key = string.Concat(values2.Value1, values2.Value2);
                return true;
            case 3:
                if (requestValues is not Request<string, string, string> values3)
                {
                    key = null;
                    return false;
                }
                key = string.Concat(values3.Value1, values3.Value2, values3.Value3);
                return true;
            case 4:
                if (requestValues is not Request<string, string, string, string> values4)
                {
                    key = null;
                    return false;
                }
                key = string.Concat(values4.Value1, values4.Value2, values4.Value3, values4.Value4);
                return true;
        }

        key = null;
        return false;
    }
}

public readonly record struct Request<T>(T Value1) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 1;
}

public readonly record struct Request<T1, T2>(T1 Value1, T2 Value2) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 2;
}

public readonly record struct Request<T1, T2, T3>(T1 Value1, T2 Value2, T3 Value3) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 3;
}

public readonly record struct Request<T1, T2, T3, T4>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 4;
}

public readonly record struct Request<T1, T2, T3, T4, T5>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 5;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 6;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6, T7>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        7 => Request.ToString(Value7),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 7;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6, T7, T8>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        7 => Request.ToString(Value7),
        8 => Request.ToString(Value8),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 8;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        7 => Request.ToString(Value7),
        8 => Request.ToString(Value8),
        9 => Request.ToString(Value9),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 9;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        7 => Request.ToString(Value7),
        8 => Request.ToString(Value8),
        9 => Request.ToString(Value9),
        10 => Request.ToString(Value10),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 10;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        7 => Request.ToString(Value7),
        8 => Request.ToString(Value8),
        9 => Request.ToString(Value9),
        10 => Request.ToString(Value10),
        11 => Request.ToString(Value11),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 11;
}

public readonly record struct Request<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => Request.ToString(Value1),
        1 => Request.ToString(Value2),
        2 => Request.ToString(Value3),
        3 => Request.ToString(Value4),
        4 => Request.ToString(Value5),
        6 => Request.ToString(Value6),
        7 => Request.ToString(Value7),
        8 => Request.ToString(Value8),
        9 => Request.ToString(Value9),
        10 => Request.ToString(Value10),
        11 => Request.ToString(Value11),
        12 => Request.ToString(Value12),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 12;
}

public readonly struct RequestValues : IRequestValues
{
    private readonly IReadOnlyList<object> _values;

    public RequestValues(IReadOnlyList<object> values)
    {
        _values = values;
    }

    public string this[int index] => Request.ToString(_values[index]);

    public int Count => _values.Count;
}
