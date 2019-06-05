using System;

namespace NetCasbin.Effect
{
    public interface IEffector
    {
        Boolean MergeEffects(String expr, NetCasbin.Effect.Effect[] effects, float[] results);
    }
}
