using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin
{
    public interface IEffector
    {
        Boolean MergeEffects(String expr, Effect[] effects, float[] results);
    }
}
