using System;
using System.Collections.Generic;
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Xunit;

namespace Casbin.UnitTests.GenericTests;

[Collection("Model collection")]
public class SupportCountTests
{
    private readonly TestModelFixture _testModelFixture;

    public SupportCountTests(TestModelFixture testModelFixture)
    {
        _testModelFixture = testModelFixture;
    }

    [Fact]
    public void TestSupportCount()
    {
        for (int i = 1; i <= 12; i++)
        {
            IEnforcer enforcer = new Enforcer(DefaultModel.CreateFromText(_testModelFixture._supportCountModelText));
            string policyType = $"p{i}";
            string requestType = $"r{i}";
            string matcherType = $"m{i}";
            if (i is 1)
            {
                policyType = PermConstants.DefaultPolicyType;
                requestType = PermConstants.DefaultRequestType;
                matcherType = PermConstants.DefaultMatcherType;
            }

            enforcer.AddNamedPolicy(policyType, CreateTestPolicy(i));
            TestEnforce(enforcer, enforcer.CreateContext(requestType, policyType,
                PermConstants.DefaultPolicyEffectType, matcherType), i);
        }
    }

    private static void TestEnforce(IEnforcer enforcer, EnforceContext context, int requestCount)
    {
        switch (requestCount)
        {
            case 1:
                Assert.True(enforcer.Enforce(context, "value1"));
                break;
            case 2:
                Assert.True(enforcer.Enforce(context, "value1", "value2"));
                break;
            case 3:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3"));
                break;
            case 4:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4"));
                break;
            case 5:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5"));
                break;
            case 6:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6"));
                break;
            case 7:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7"));
                break;
            case 8:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8"));
                break;
            case 9:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9"));
                break;
            case 10:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10"));
                break;
            case 11:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10", "value11"));
                break;
            case 12:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10", "value11", "value12"));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requestCount));
        }
    }

    private static IPolicyValues CreateTestPolicy(int count)
    {
        var policy = new List<string>();
        for (int i = 0; i < count; i++)
        {
            policy.Add($"value{i + 1}");
        }

        return Policy.ValuesFrom(policy);
    }
}
