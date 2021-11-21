using System.Collections.Generic;

namespace Casbin.Effect
{
    /// <summary>
    /// Effector is the interface for Casbin effectors.
    /// </summary>
    public interface IEffector
    {
        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpression">The expression of [policy_effect].</param>
        /// <param name="effects">The effects of all matched rules.</param>
        /// <param name="results">The matcher results of all matched rules.</param>
        /// <param name="policyCount"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <param name="policyIndex"></param>
        /// <returns>The final effect.</returns>
        PolicyEffect MergeEffects(string effectExpression, IReadOnlyList<PolicyEffect>  effects, IReadOnlyList<float> results, int policyIndex, int policyCount, out int hitPolicyIndex);
    }
}
