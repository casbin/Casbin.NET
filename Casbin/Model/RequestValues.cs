using System;
using System.Collections.Generic;

namespace Casbin.Model;

public readonly record struct RequestValues : IRequestValues
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

public readonly record struct RequestValues<T>(T Value1) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 1;
}

public readonly record struct RequestValues<T1, T2>(T1 Value1, T2 Value2) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 2;
}

public readonly record struct RequestValues<T1, T2, T3>(T1 Value1, T2 Value2, T3 Value3) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 3;
}

public readonly record struct RequestValues<T1, T2, T3, T4>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4) : IRequestValues
{
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

public readonly record struct RequestValues<T1, T2, T3, T4, T5>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5) : IRequestValues
{
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

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 6;
}

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6, T7>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        7 => RequestValues.ToStringValue(Value7),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 7;
}

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        7 => RequestValues.ToStringValue(Value7),
        8 => RequestValues.ToStringValue(Value8),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 8;
}

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        7 => RequestValues.ToStringValue(Value7),
        8 => RequestValues.ToStringValue(Value8),
        9 => RequestValues.ToStringValue(Value9),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 9;
}

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 Value1, T2 Value2, T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        7 => RequestValues.ToStringValue(Value7),
        8 => RequestValues.ToStringValue(Value8),
        9 => RequestValues.ToStringValue(Value9),
        10 => RequestValues.ToStringValue(Value10),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 10;
}

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 Value1, T2 Value2,
    T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        7 => RequestValues.ToStringValue(Value7),
        8 => RequestValues.ToStringValue(Value8),
        9 => RequestValues.ToStringValue(Value9),
        10 => RequestValues.ToStringValue(Value10),
        11 => RequestValues.ToStringValue(Value11),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 11;
}

public readonly record struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 Value1, T2 Value2,
    T3 Value3,
    T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11,
    T12 Value12) : IRequestValues
{
    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        6 => RequestValues.ToStringValue(Value6),
        7 => RequestValues.ToStringValue(Value7),
        8 => RequestValues.ToStringValue(Value8),
        9 => RequestValues.ToStringValue(Value9),
        10 => RequestValues.ToStringValue(Value10),
        11 => RequestValues.ToStringValue(Value11),
        12 => RequestValues.ToStringValue(Value12),
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
