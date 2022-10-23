using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Model;

namespace Casbin.Persist;

[Obsolete("Please use PolicyFilter instead")]
public class Filter : IPolicyFilter
{
    private readonly PolicyFilter _filter;

    public Filter() => _filter = this;

    public IEnumerable<string> G { get; set; }

    public IEnumerable<string> P { get; set; }

    public IQueryable<IPersistantPolicy> ApplyFilter(IQueryable<IPersistantPolicy> policies) =>
        _filter is not null ? _filter.ApplyFilter(policies) : policies;

    public static implicit operator PolicyFilter(Filter filter)
    {
        if (filter is null)
        {
            return PolicyFilter.Empty;
        }

        if (filter.P is null && filter.G is null)
        {
            return PolicyFilter.Empty;
        }

        if (filter.P is not null && filter.G is not null)
        {
            PolicyFilter filterP = new(PermConstants.DefaultPolicyType, 0,
                Policy.CreateValues(filter.P));
            PolicyFilter filterG = new(PermConstants.DefaultGroupingPolicyType, 0,
                Policy.CreateValues(filter.G));
            return filterP.Or(filterG) as PolicyFilter;
        }

        if (filter.P is not null)
        {
            return new PolicyFilter(PermConstants.DefaultPolicyType, 0,
                Policy.CreateValues(filter.P));
        }

        if (filter.G is not null)
        {
            return new PolicyFilter(PermConstants.DefaultPolicyType, 0,
                Policy.CreateValues(filter.P));
        }

        return PolicyFilter.Empty;
    }
}


