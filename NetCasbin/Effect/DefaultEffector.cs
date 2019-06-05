using System;

namespace NetCasbin.Effect
{
    public class DefaultEffector : IEffector
    {
        public bool MergeEffects(string expr, NetCasbin.Effect.Effect[] effects, float[] results)
        {
            bool result = false;
            if (expr.Equals("some(where (p_eft == allow))"))
            {
                foreach (var eft in effects)
                {
                    if (eft == NetCasbin.Effect.Effect.Allow)
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
                    if (eft == NetCasbin.Effect.Effect.Deny)
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
                    if (eft == NetCasbin.Effect.Effect.Allow)
                    {
                        result = true;
                    }
                    else if (eft == NetCasbin.Effect.Effect.Deny)
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
                    if (eft != NetCasbin.Effect.Effect.Indeterminate)
                    {
                        if (eft == NetCasbin.Effect.Effect.Allow)
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
