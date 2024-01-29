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

    internal static PolicyValues CreateValues(IList<string> values) => new(PolicyValues.ToText(values), values);

    internal static PolicyValues CreateValues(params object[] objs) => new(objs);

    public static IPolicyValues ValuesFrom(IList<string> values)
    {
        return CreateValues(values);
    }

    public static IPolicyValues ValuesFrom(IEnumerable<string> values) =>
        ValuesFrom(values as IList<string> ?? values.ToArray());

    public static IPolicyValues ValuesFrom(IPersistPolicy values)
    {
        // Find the latest not empty value as the count.
        int count;
        if (!(values.Value12 is null))
        {
            count = 12;
        }
        else if (!(values.Value11 is null))
        {
            count = 11;
        }
        else if (!(values.Value10 is null))
        {
            count = 10;
        }
        else if (!(values.Value9 is null))
        {
            count = 9;
        }
        else if (!(values.Value8 is null))
        {
            count = 8;
        }
        else if (!(values.Value7 is null))
        {
            count = 7;
        }
        else if (!(values.Value6 is null))
        {
            count = 6;
        }
        else if (!(values.Value5 is null))
        {
            count = 5;
        }
        else if (!(values.Value4 is null))
        {
            count = 4;
        }
        else if (!(values.Value3 is null))
        {
            count = 3;
        }
        else if (!(values.Value2 is null))
        {
            count = 2;
        }
        else if (!(values.Value1 is null))
        {
            count = 1;
        }
        else
        {
            count = 0;
        }

        return count switch
        {
            1 => CreateValues(new List<string>() { values.Value1 }),
            2 => CreateValues(new List<string>() { values.Value1, values.Value2 }),
            3 => CreateValues(new List<string>() { values.Value1, values.Value2, values.Value3 }),
            4 => CreateValues(new List<string>() { values.Value1, values.Value2, values.Value3, values.Value4 }),
            5 => CreateValues(new List<string>() { values.Value1, values.Value2, values.Value3, values.Value4, values.Value5 }),
            6 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6 }),
            7 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7 }),
            8 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8 }),
            9 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9 }),
            10 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10 }),
            11 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11 }),
            12 => CreateValues(new List<string>() {values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11,
                values.Value12 }),
            _ => throw new ArgumentOutOfRangeException(nameof(count), count, null)
        };
    }


    public static IReadOnlyList<IPolicyValues> ValuesListFrom(IEnumerable<IEnumerable<string>> rules)
    {
        IEnumerable<IPolicyValues> policies = rules.Select(ValuesFrom);
        return policies as IReadOnlyList<IPolicyValues> ?? policies.ToArray();
    }
}


