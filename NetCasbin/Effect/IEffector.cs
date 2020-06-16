using System;

namespace NetCasbin.Effect
{
    /// <summary>
    /// Effector is the interface for Casbin effectors.
    /// </summary>
    public interface IEffector
    {
        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="expr">The expression of [policy_effect].</param>
        /// <param name="effects">The effects of all matched rules.</param>
        /// <param name="results">The matcher results of all matched rules.</param>
        /// <returns>The final effect.</returns>
        bool MergeEffects(string expr, Effect[] effects, float[] results);
    }
}
