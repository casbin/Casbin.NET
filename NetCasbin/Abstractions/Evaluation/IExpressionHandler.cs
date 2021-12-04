using System;

namespace Casbin.Evaluation;

public interface IExpressionHandler
{
    public void SetFunction(string name, Delegate function);

    public bool Invoke(in EnforceContext context, ref EnforceSession session);
}
