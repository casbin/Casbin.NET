using System;

namespace NetCasbin.Effect
{
    /// <summary>
    /// DefaultEffector is default effector for Casbin.
    /// </summary>
    public class DefaultEffector : IEffector
    {
        /// <summary>
        ///  mergeEffects merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="effects"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public bool MergeEffects(string expr, Effect[] effects, float[] results)
        {
            var result = false;
            if (expr.Equals("some(where (p_eft == allow))"))
            {
                foreach (var eft in effects)
                {
                    if (eft == Effect.Allow)
                    {
                        result = true;
                        break;
                    }
                }
            }
            else if (expr.Equals("!some(where (p_eft == deny))"))
            {
                result = true;

                foreach (var eft in effects)
                {
                    if (eft == Effect.Deny)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else if (expr.Equals("some(where (p_eft == allow)) && !some(where (p_eft == deny))"))
            {
                result = false;
                foreach (var eft in effects)
                {
                    if (eft == Effect.Allow)
                    {
                        result = true;
                    }
                    else if (eft == Effect.Deny)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else if (expr.Equals("priority(p_eft) || deny"))
            {
                result = false;
                foreach (var eft in effects)
                {
                    if (eft != Effect.Indeterminate)
                    {
                        if (eft == Effect.Allow)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("unsupported effect");
            }
            return result;
        }
    }
}
