using System.Collections.Generic;
using Casbin.Model;
using Xunit;

namespace Casbin.UnitTests.Extensions;

internal static class ManagementEnforcerExtension
{
    internal static void TestGetPolicy(this IEnforcer e, IReadOnlyList<IPolicyValues> exceptedValues)
    {
        IEnumerable<IEnumerable<string>> actualValues = e.GetPolicy();
        Assert.True(exceptedValues.DeepEquals(actualValues));
    }
}
