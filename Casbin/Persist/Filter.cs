using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Model;

namespace Casbin.Persist;

[Obsolete("Please use PolicyFilter instead")]
public class Filter : IPolicyFilter
{
    private PolicyFilter _filterG;


    private PolicyFilter _filterP;
    private IEnumerable<string> _g;
    private IEnumerable<string> _p;

    public IEnumerable<string> P
    {
        get => _p;
        set
        {
            _p = value;
            _filterP = new PolicyFilter(PermConstants.DefaultPolicyType, 0,
                Policy.CreateValues(value));
        }
    }

    public IEnumerable<string> G
    {
        get => _g;
        set
        {
            _g = value;
            _filterG = new PolicyFilter(PermConstants.DefaultGroupingPolicyType, 0,
                Policy.CreateValues(value));
        }
    }

    public IQueryable<T> Apply<T>(IQueryable<T> policies) where T : IPersistPolicy
    {
        if (_filterP is null && _filterG is null)
        {
            return policies;
        }

        if (_filterP is not null && _filterG is not null)
        {
            IQueryable<T> policiesP = _filterP.Apply(policies);
            IQueryable<T> policiesG = _filterG.Apply(policies);
            return policiesP.Union(policiesG);
        }

        if (_filterP is not null)
        {
            return _filterP.Apply(policies);
        }

        if (_filterG is not null)
        {
            return _filterG.Apply(policies);
        }

        return policies;
    }
}





