﻿using System;
using System.Linq;
using Casbin.Model;

namespace Casbin.Persist;

public class PolicyFilter : IPolicyFilter
{
    public static readonly PolicyFilter Empty = new();
    private readonly int _fieldIndex;

    private readonly string _policyType;
    private readonly IPolicyValues _values;

    public PolicyFilter()
    {
    }

    public PolicyFilter(string policyType, int fieldIndex, IPolicyValues values)
    {
        _policyType = policyType;
        _fieldIndex = fieldIndex;
        _values = values;
    }

    public IQueryable<T> Apply<T>(IQueryable<T> policies)
        where T : IPersistPolicy
    {
        if (_policyType is null)
        {
            return policies;
        }

        if (_values is null)
        {
            return policies;
        }

        return FilterValues(policies, _policyType, _fieldIndex, _values);
    }

    private static IQueryable<T> FilterValues<T>(IQueryable<T> query,
        string policyType, int fieldIndex, IPolicyValues values)
        where T : IPersistPolicy
    {
        if (fieldIndex > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(fieldIndex));
        }

        int fieldValueCount = values.Count;
        if (fieldValueCount is 0)
        {
            return query;
        }

        int lastIndex = fieldIndex + fieldValueCount - 1;

        if (lastIndex > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(lastIndex));
        }

        query = query.Where(p => string.Equals(p.Type, policyType));

        if (fieldIndex is 0 && lastIndex >= 0)
        {
            string field = values[fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value1 == field);
            }
        }

        if (fieldIndex <= 1 && lastIndex >= 1)
        {
            string field = values[1 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value2 == field);
            }
        }

        if (fieldIndex <= 2 && lastIndex >= 2)
        {
            string field = values[2 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value3 == field);
            }
        }

        if (fieldIndex <= 3 && lastIndex >= 3)
        {
            string field = values[3 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value4 == field);
            }
        }

        if (fieldIndex <= 4 && lastIndex >= 4)
        {
            string field = values[4 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value5 == field);
            }
        }

        if (fieldIndex <= 5 && lastIndex >= 5)
        {
            string field = values[5 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value6 == field);
            }
        }

        if (fieldIndex <= 6 && lastIndex >= 6)
        {
            string field = values[6 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value7 == field);
            }
        }

        if (fieldIndex <= 7 && lastIndex >= 7)
        {
            string field = values[7 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value8 == field);
            }
        }

        if (fieldIndex <= 8 && lastIndex >= 8)
        {
            string field = values[8 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value9 == field);
            }
        }

        if (fieldIndex <= 9 && lastIndex >= 9)
        {
            string field = values[9 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value10 == field);
            }
        }

        if (fieldIndex <= 10 && lastIndex >= 10)
        {
            string field = values[5 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value11 == field);
            }
        }

        if (lastIndex is 11) // and fieldIndex <= 11
        {
            string field = values[11 - fieldIndex];
            if (string.IsNullOrWhiteSpace(field) is false)
            {
                query = query.Where(p => p.Value12 == field);
            }
        }

        return query;
    }
}



