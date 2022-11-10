using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Persist;

namespace Casbin.Model;

public static class Policy
{
    public static bool SupportGeneric(int count)
    {
        return count is >= 1 and <= 12;
    }

    public static PolicyValues<T1> CreateValues<T1>(T1 value1) => new(value1);

    public static PolicyValues<T1, T2> CreateValues<T1, T2>(T1 value1, T2 value2) => new(value1, value2);

    public static PolicyValues<T1, T2, T3> CreateValues<T1, T2, T3>(T1 value1, T2 value2, T3 value3) =>
        new(value1, value2, value3);

    public static PolicyValues<T1, T2, T3, T4>
        CreateValues<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) =>
        new(value1, value2, value3, value4);

    public static PolicyValues<T1, T2, T3, T4, T5> CreateValues<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5) => new(value1, value2, value3, value4, value5);

    public static PolicyValues<T1, T2, T3, T4, T5, T6> CreateValues<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2,
        T3 value3,
        T4 value4, T5 value5, T6 value6) => new(value1, value2, value3, value4, value5, value6);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7> CreateValues<T1, T2, T3, T4, T5, T6, T7>(T1 value1,
        T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) =>
        new(value1, value2, value3, value4, value5, value6, value7);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1,
        T2 value2,
        T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) =>
        new(value1, value2, value3, value4, value5, value6, value7,
            value8);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        T1 value1,
        T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9) =>
        new(value1, value2, value3, value4, value5, value6, value7,
            value8, value9);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9,
        T10>(
        T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10) =>
        new(value1, value2, value3, value4, value5, value6,
            value7, value8, value9, value10);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> CreateValues<T1, T2, T3, T4, T5, T6, T7,
        T8, T9,
        T10,
        T11>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11) =>
        new(value1, value2, value3, value4, value5, value6,
            value7, value8, value9, value10, value11);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> CreateValues<T1, T2, T3, T4, T5, T6,
        T7, T8,
        T9,
        T10, T11, T12>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12) =>
        new(value1, value2, value3, value4, value5,
            value6, value7, value8, value9, value10, value11, value12);


    internal static PolicyValues<string> CreateValues(IReadOnlyList<string> values, string value1)
        => new(PolicyValues.ToText(values), value1, value1);

    internal static PolicyValues<string, string> CreateValues(IReadOnlyList<string> values, string value1,
        string value2)
        => new(PolicyValues.ToText(values), value1, value1, value2, value2);

    internal static PolicyValues<string, string, string> CreateValues(IReadOnlyList<string> values, string value1,
        string value2, string value3)
        => new(PolicyValues.ToText(values), value1, value1, value2, value2, value3, value3);

    internal static PolicyValues<string, string, string, string> CreateValues(IReadOnlyList<string> values,
        string value1,
        string value2, string value3, string value4)
        => new(PolicyValues.ToText(values), value1, value1, value2, value2, value3, value3, value4, value4);

    internal static PolicyValues<string, string, string, string, string> CreateValues(IReadOnlyList<string> values,
        string value1, string value2, string value3, string value4, string value5)
        => new(PolicyValues.ToText(values), value1, value1, value2, value2, value3, value3, value4, value4, value5,
            value5);

    internal static PolicyValues<string, string, string, string, string, string> CreateValues(
        IReadOnlyList<string> values,
        string value1, string value2, string value3, string value4, string value5, string value6)
        => new(PolicyValues.ToText(values), value1, value1, value2, value2, value3, value3, value4, value4, value5,
            value5, value6,
            value6);

    internal static PolicyValues<string, string, string, string, string, string, string> CreateValues(
        IReadOnlyList<string> values, string value1, string value2, string value3, string value4, string value5,
        string value6, string value7)
        => new(PolicyValues.ToText(values), value1, value1, value2, value2, value3, value3, value4, value4, value5,
            value5, value6,
            value6, value7, value7);

    public static IPolicyValues ValuesFrom(IReadOnlyList<string> values)
    {
        return values.Count switch
        {
            1 => CreateValues(values, values[0]),
            2 => CreateValues(values, values[0], values[1]),
            3 => CreateValues(values, values[0], values[1], values[2]),
            4 => CreateValues(values, values[0], values[1], values[2], values[3]),
            5 => CreateValues(values, values[0], values[1], values[2], values[3], values[4]),
            6 => CreateValues(values, values[0], values[1], values[2], values[3], values[4], values[5]),
            7 => CreateValues(values, values[0], values[1], values[2], values[3], values[4], values[5], values[6]),
            8 => CreateValues(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7]),
            9 => CreateValues(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8]),
            10 => CreateValues(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8],
                values[9]),
            11 => CreateValues(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8],
                values[9], values[10]),
            12 => CreateValues(values[0], values[1], values[2], values[3],
                values[4], values[5], values[6], values[7], values[8],
                values[9], values[10], values[11]),
            _ => new StringListPolicyValues(values)
        };
    }

    public static IPolicyValues ValuesFrom(IEnumerable<string> values) =>
        ValuesFrom(values as IReadOnlyList<string> ?? values.ToArray());

    public static IPolicyValues ValuesFrom(IPersistPolicy values)
    {
        // Find the latest not empty value as the count.
        int count;
        if (string.IsNullOrWhiteSpace(values.Value12) is false)
        {
            count = 12;
        }
        else if (string.IsNullOrWhiteSpace(values.Value11) is false)
        {
            count = 11;
        }
        else if (string.IsNullOrWhiteSpace(values.Value10) is false)
        {
            count = 10;
        }
        else if (string.IsNullOrWhiteSpace(values.Value9) is false)
        {
            count = 9;
        }
        else if (string.IsNullOrWhiteSpace(values.Value8) is false)
        {
            count = 8;
        }
        else if (string.IsNullOrWhiteSpace(values.Value7) is false)
        {
            count = 7;
        }
        else if (string.IsNullOrWhiteSpace(values.Value6) is false)
        {
            count = 6;
        }
        else if (string.IsNullOrWhiteSpace(values.Value5) is false)
        {
            count = 5;
        }
        else if (string.IsNullOrWhiteSpace(values.Value4) is false)
        {
            count = 4;
        }
        else if (string.IsNullOrWhiteSpace(values.Value3) is false)
        {
            count = 3;
        }
        else if (string.IsNullOrWhiteSpace(values.Value2) is false)
        {
            count = 2;
        }
        else if (string.IsNullOrWhiteSpace(values.Value1) is false)
        {
            count = 1;
        }
        else
        {
            count = 0;
        }

        return count switch
        {
            1 => CreateValues(values.Value1),
            2 => CreateValues(values.Value1, values.Value2),
            3 => CreateValues(values.Value1, values.Value2, values.Value3),
            4 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4),
            5 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5),
            6 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6),
            7 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7),
            8 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8),
            9 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9),
            10 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10),
            11 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11),
            12 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11,
                values.Value12),
            _ => throw new ArgumentOutOfRangeException(nameof(count), count, null)
        };
    }

    public static IReadOnlyList<IPolicyValues> ValuesListFrom(IEnumerable<IEnumerable<string>> rules)
    {
        IEnumerable<IPolicyValues> policies = rules.Select(ValuesFrom);
        return policies as IReadOnlyList<IPolicyValues> ?? policies.ToArray();
    }
}


