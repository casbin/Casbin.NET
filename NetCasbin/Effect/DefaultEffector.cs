using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin
{
    public class DefaultEffector : IEffector
    {
        public bool MergeEffects(string expr, Effect[] effects, float[] results)
        {
            bool result = false;
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
