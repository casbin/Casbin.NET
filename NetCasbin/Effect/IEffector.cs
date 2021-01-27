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
        /// <param name="policyEffect">The expression of [policy_effect].</param>
        /// <param name="effects">The effects of all matched rules.</param>
        /// <param name="results">The matcher results of all matched rules.</param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns>The final effect.</returns>
        bool MergeEffects(string policyEffect, Effect[] effects, float[] results, out int hitPolicyIndex);
    }
}
