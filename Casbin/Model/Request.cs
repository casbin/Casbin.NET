﻿using System.Collections.Generic;

namespace Casbin.Model;

public static class Request
{
    public static bool SupportGeneric(int count)
    {
        return count is >= 1 and <= 12;
    }

    public static RequestValues<T> CreateValues<T>(T value)
    {
        return new RequestValues<T>(value);
    }

    public static RequestValues<T1, T2> CreateValues<T1, T2>(T1 value1, T2 value2)
    {
        return new RequestValues<T1, T2>(value1, value2);
    }

    public static RequestValues<T1, T2, T3> CreateValues<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
    {
        return new RequestValues<T1, T2, T3>(value1, value2, value3);
    }

    public static RequestValues<T1, T2, T3, T4> CreateValues<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3,
        T4 value4)
    {
        return new RequestValues<T1, T2, T3, T4>(value1, value2, value3,
            value4);
    }

    public static RequestValues<T1, T2, T3, T4, T5> CreateValues<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5)
    {
        return new RequestValues<T1, T2, T3, T4, T5>(value1, value2, value3,
            value4, value5);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6> CreateValues<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2,
        T3 value3,
        T4 value4, T5 value5, T6 value6)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6>(value1, value2, value3,
            value4, value5, value6);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6, T7> CreateValues<T1, T2, T3, T4, T5, T6, T7>(T1 value1,
        T2 value2,
        T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6, T7>(value1, value2, value3,
            value4, value5, value6, value7);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6, T7, T8> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1,
        T2 value2,
        T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6, T7, T8>(value1, value2, value3,
            value4, value5, value6, value7, value8);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        T1 value1,
        T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8,
        T9, T10>(
        T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9, value10);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> CreateValues<T1, T2, T3, T4, T5, T6, T7,
        T8, T9, T10,
        T11>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9, value10, value11);
    }

    public static RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> CreateValues<T1, T2, T3, T4, T5, T6,
        T7, T8, T9,
        T10, T11, T12>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12)
    {
        return new RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(value1, value2, value3,
            value4, value5, value6, value7, value8, value9, value10, value11, value12);
    }

    public static ListRequestValues<T> CreateValues<T>(params T[] value)
    {
        return new ListRequestValues<T>(value);
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
                if (requestValues is not RequestValues<string> value)
                {
                    key = null;
                    return false;
                }

                key = value.Value1;
                return true;
            case 2:
                if (requestValues is not RequestValues<string, string> values2)
                {
                    key = null;
                    return false;
                }

                key = string.Concat(values2.Value1, values2.Value2);
                return true;
            case 3:
                if (requestValues is not RequestValues<string, string, string> values3)
                {
                    key = null;
                    return false;
                }

                key = string.Concat(values3.Value1, values3.Value2, values3.Value3);
                return true;
            case 4:
                if (requestValues is not RequestValues<string, string, string, string> values4)
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
