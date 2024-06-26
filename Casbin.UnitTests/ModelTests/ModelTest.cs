﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public partial class ModelTest
{
    private readonly TestModelFixture TestModelFixture;

    public ModelTest(TestModelFixture testModelFixture) => TestModelFixture = testModelFixture;

    [Fact]
    public void TestBasicModel()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestBasicModelNoPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.BasicModelText));

        TestEnforce(e, "alice", "data1", "read", false);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", false);
    }

    [Fact]
    public void TestBasicModelWithRoot()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.BasicWithRootModelText,
            TestModelFixture.BasicPolicyText));

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
        TestEnforce(e, "root", "data1", "read", true);
        TestEnforce(e, "root", "data1", "write", true);
        TestEnforce(e, "root", "data2", "read", true);
        TestEnforce(e, "root", "data2", "write", true);
    }

    [Fact]
    public void TestBasicModelWithRootNoPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.BasicWithRootModelText));

        TestEnforce(e, "alice", "data1", "read", false);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", false);
        TestEnforce(e, "root", "data1", "read", true);
        TestEnforce(e, "root", "data1", "write", true);
        TestEnforce(e, "root", "data2", "read", true);
        TestEnforce(e, "root", "data2", "write", true);
    }

    [Fact]
    public void TestBasicModelWithoutUsers()
    {
        Enforcer e = new(TestModelFixture.GetBasicWithoutUserTestModel());

        TestEnforceWithoutUsers(e, "data1", "read", true);
        TestEnforceWithoutUsers(e, "data1", "write", false);
        TestEnforceWithoutUsers(e, "data2", "read", false);
        TestEnforceWithoutUsers(e, "data2", "write", true);
    }

    [Fact]
    public void TestBasicModelWithoutResources()
    {
        Enforcer e = new(TestModelFixture.GetBasicWithoutResourceTestModel());

        TestEnforceWithoutUsers(e, "alice", "read", true);
        TestEnforceWithoutUsers(e, "alice", "write", false);
        TestEnforceWithoutUsers(e, "bob", "read", false);
        TestEnforceWithoutUsers(e, "bob", "write", true);
    }

    [Fact]
    public void TestRbacModel()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelWithResourceRoles()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithResourceRoleTestModel());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", true);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", true);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelWithDomainsAtRuntime()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.RbacWithDomainsModelText));
        e.BuildRoleLinks();

        e.AddPolicy("admin", "domain1", "data1", "read");
        e.AddPolicy("admin", "domain1", "data1", "write");
        e.AddPolicy("admin", "domain2", "data2", "read");
        e.AddPolicy("admin", "domain2", "data2", "write");

        e.AddGroupingPolicy("alice", "admin", "domain1");
        e.AddGroupingPolicy("bob", "admin", "domain2");

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", true);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

        // Remove all policy rules related to domain1 and data1.
        e.RemoveFilteredPolicy(1, "domain1", "data1");

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

        // Remove the specified policy rule.
        e.RemovePolicy("admin", "domain2", "data2", "read");

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);
    }

    [Fact]
    public async Task TestRbacModelWithDomainsAtRuntimeAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.RbacWithDomainsModelText));
        e.BuildRoleLinks();

        await e.AddPolicyAsync("admin", "domain1", "data1", "read");
        await e.AddPolicyAsync("admin", "domain1", "data1", "write");
        await e.AddPolicyAsync("admin", "domain2", "data2", "read");
        await e.AddPolicyAsync("admin", "domain2", "data2", "write");

        await e.AddGroupingPolicyAsync("alice", "admin", "domain1");
        await e.AddGroupingPolicyAsync("bob", "admin", "domain2");

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", true);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

        // Remove all policy rules related to domain1 and data1.
        await e.RemoveFilteredPolicyAsync(1, "domain1", "data1");

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

        // Remove the specified policy rule.
        await e.RemovePolicyAsync("admin", "domain2", "data2", "read");

        TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
        TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "read", false);
        TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelWithDeny()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDenyTestModel());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelWithOnlyDeny()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithNotDenyModelText,
            TestModelFixture.RbacWithDenyPolicyText));
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data2", "write", false);
    }

    [Fact]
    public void TestRbacModelWithCustomData()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
        // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
        // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
        e.AddGroupingPolicy("bob", "data2_admin", "custom_data");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", true);
        TestEnforce(e, "bob", "data2", "write", true);

        // You should also take the custom data as a parameter when deleting a grouping policy.
        // e.removeGroupingPolicy("bob", "data2_admin") won't work.
        // Or you can remove it by using removeFilteredGroupingPolicy().
        e.RemoveGroupingPolicy("bob", "data2_admin", "custom_data");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public async Task TestRbacModelWithCustomDataAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
        // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
        // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
        await e.AddGroupingPolicyAsync("bob", "data2_admin", "custom_data");

        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", true);
        await TestEnforceAsync(e, "alice", "data2", "write", true);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", true);
        await TestEnforceAsync(e, "bob", "data2", "write", true);

        // You should also take the custom data as a parameter when deleting a grouping policy.
        // e.removeGroupingPolicy("bob", "data2_admin") won't work.
        // Or you can remove it by using removeFilteredGroupingPolicy().
        await e.RemoveGroupingPolicyAsync("bob", "data2_admin", "custom_data");

        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", true);
        await TestEnforceAsync(e, "alice", "data2", "write", true);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", false);
        await TestEnforceAsync(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelWithCustomRoleManager()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.SetRoleManager(new MockCustomRoleManager());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestAbacModel()
    {
        Enforcer e = new(TestModelFixture.GetNewAbacModel());

        TestResource data1 = new("data1", "alice");
        TestResource data2 = new("data2", "bob");

        TestEnforce(e, "alice", data1, "read", true);
        TestEnforce(e, "alice", data1, "write", true);
        TestEnforce(e, "alice", data2, "read", false);
        TestEnforce(e, "alice", data2, "write", false);
        TestEnforce(e, "bob", data1, "read", false);
        TestEnforce(e, "bob", data1, "write", false);
        TestEnforce(e, "bob", data2, "read", true);
        TestEnforce(e, "bob", data2, "write", true);
    }

    [Fact]
    public void TestAbacWithEvalModel()
    {
        Enforcer e = new(TestModelFixture.GetNewAbacWithEvalModel());
        TestSubject subject1 = new("alice", 16);
        TestSubject subject2 = new("alice", 20);
        TestSubject subject3 = new("alice", 65);

        TestEnforce(e, subject1, "/data1", "read", false);
        TestEnforce(e, subject1, "/data2", "read", false);
        TestEnforce(e, subject1, "/data1", "write", false);
        TestEnforce(e, subject1, "/data2", "write", true);

        TestEnforce(e, subject2, "/data1", "read", true);
        TestEnforce(e, subject2, "/data2", "read", false);
        TestEnforce(e, subject2, "/data1", "write", false);
        TestEnforce(e, subject2, "/data2", "write", true);

        TestEnforce(e, subject3, "/data1", "read", true);
        TestEnforce(e, subject3, "/data2", "read", false);
        TestEnforce(e, subject3, "/data1", "write", false);
        TestEnforce(e, subject3, "/data2", "write", false);
    }

    [Fact]
    public void TestKeyMatchModel()
    {
        Enforcer e = new(TestModelFixture.GetNewKeyMatchTestModel());

        TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        TestEnforce(e, "alice", "/alice_data/resource1", "POST", true);
        TestEnforce(e, "alice", "/alice_data/resource2", "GET", true);
        TestEnforce(e, "alice", "/alice_data/resource2", "POST", false);
        TestEnforce(e, "alice", "/bob_data/resource1", "GET", false);
        TestEnforce(e, "alice", "/bob_data/resource1", "POST", false);
        TestEnforce(e, "alice", "/bob_data/resource2", "GET", false);
        TestEnforce(e, "alice", "/bob_data/resource2", "POST", false);

        TestEnforce(e, "bob", "/alice_data/resource1", "GET", false);
        TestEnforce(e, "bob", "/alice_data/resource1", "POST", false);
        TestEnforce(e, "bob", "/alice_data/resource2", "GET", true);
        TestEnforce(e, "bob", "/alice_data/resource2", "POST", false);
        TestEnforce(e, "bob", "/bob_data/resource1", "GET", false);
        TestEnforce(e, "bob", "/bob_data/resource1", "POST", true);
        TestEnforce(e, "bob", "/bob_data/resource2", "GET", false);
        TestEnforce(e, "bob", "/bob_data/resource2", "POST", true);

        TestEnforce(e, "cathy", "/cathy_data", "GET", true);
        TestEnforce(e, "cathy", "/cathy_data", "POST", true);
        TestEnforce(e, "cathy", "/cathy_data", "DELETE", false);
    }

    [Fact]
    public void TestPriorityModelIndeterminate()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.PriorityModelText,
            TestModelFixture.PriorityIndeterminatePolicyText));
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "read", false);
    }

    [Fact]
    public void TestPriorityModel()
    {
        Enforcer e = new(TestModelFixture.GetNewPriorityTestModel());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", true);
        TestEnforce(e, "bob", "data2", "write", false);
    }

    [Fact]
    public void TestPriorityExplicitModel()
    {
        Enforcer e = new(TestModelFixture.GetNewPriorityExplicitTestModel());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data1", "write", true);
        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
        TestEnforce(e, "data1_deny_group", "data1", "read", false);
        TestEnforce(e, "data1_deny_group", "data1", "write", false);
        TestEnforce(e, "data2_allow_group", "data2", "read", true);
        TestEnforce(e, "data2_allow_group", "data2", "write", true);

        // add a higher priority policy
        e.AddPolicy("1", "bob", "data2", "write", "deny");

        TestEnforce(e, "alice", "data1", "write", true);
        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", false);
        TestEnforce(e, "data1_deny_group", "data1", "read", false);
        TestEnforce(e, "data1_deny_group", "data1", "write", false);
        TestEnforce(e, "data2_allow_group", "data2", "read", true);
        TestEnforce(e, "data2_allow_group", "data2", "write", true);
    }

    [Fact]
    public void TestPriorityExplicitDenyOverrideModel()
    {
        Enforcer e = new(TestModelFixture.GetNewPriorityExplicitDenyOverrideModel());
        e.BuildRoleLinks();

        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data2", "read", true);

        // adding a new group, simulating behaviour when two different groups are added to the same person.
        e.AddPolicy("10", "data2_deny_group_new", "data2", "write", "deny");
        e.AddGroupingPolicy("alice", "data2_deny_group_new");

        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data2", "read", true);

        // expected enforcement result should be true,
        // as there is a policy with a lower rank 10, that produces allow result.
        e.AddPolicy("5", "alice", "data2", "write", "allow");
        TestEnforce(e, "alice", "data2", "write", true);

        // adding deny policy for alice for the same obj,
        // to ensure that if there is at least one deny, final result will be deny.
        e.AddPolicy("5", "alice", "data2", "write", "deny");
        TestEnforce(e, "alice", "data2", "write", false);

        // adding higher fake higher priority policy for alice,
        // expected enforcement result should be true (ignore this policy).
        e.AddPolicy("2", "alice", "data2", "write", "allow");
        TestEnforce(e, "alice", "data2", "write", true);
        e.AddPolicy("1", "fake-subject", "fake-object", "very-fake-action", "allow");
        TestEnforce(e, "alice", "data2", "write", true);

        // adding higher (less of 0) priority policy for alice,
        // to override group policies again.
        e.AddPolicy("-1", "alice", "data2", "write", "deny");
        TestEnforce(e, "alice", "data2", "write", false);
    }

    [Fact]
    public void TestKeyMatch2Model()
    {
        Enforcer e = new(TestModelFixture.GetNewKeyMatch2TestModel());

        TestEnforce(e, "alice", "/alice_data", "GET", false);
        TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        TestEnforce(e, "alice", "/alice_data2/myid", "GET", false);
        TestEnforce(e, "alice", "/alice_data2/myid/using/res_id", "GET", true);
    }

    [Fact]
    public void TestKeyMatchCustomModel()
    {
        static bool CustomFunction(string key1, string key2)
        {
            return (key1 is "/alice_data2/myid/using/res_id" && key2 is "/alice_data/:resource")
                   || (key1 is "/alice_data2/myid/using/res_id" && key2 is "/alice_data2/:id/using/:resId");
        }

        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.KeyMatchCustomModelText,
            TestModelFixture.KeyMatch2PolicyText));

        e.AddFunction("keyMatchCustom", CustomFunction);

        TestEnforce(e, "alice", "/alice_data2/myid", "GET", false);
        TestEnforce(e, "alice", "/alice_data2/myid/using/res_id", "GET", true);
    }

    [Fact]
    public void TestMultipleTypeModel()
    {
        Enforcer e = new(TestModelFixture.GetNewMultipleTypeTestModel());
        e.BuildRoleLinks();

        // Use default types
        EnforceContext context = e.CreateContext();

        Assert.True(e.Enforce(context, "alice", "data1", "read"));
        Assert.False(e.Enforce(context, "alice", "data1", "write"));

        Assert.True(e.Enforce(context, "bob", "data2", "read"));
        Assert.False(e.Enforce(context, "bob", "data2", "write"));

        // Use r2 p2 and m2 type
        context = e.CreateContext
        (
            PermConstants.RequestType2,
            PermConstants.PolicyType2,
            PermConstants.DefaultPolicyEffectType,
            PermConstants.MatcherType2
        );

        Assert.True(e.Enforce(context, "alice", "domain1", "data2", "read"));
        Assert.False(e.Enforce(context, "alice", "domain1", "data2", "write"));

        Assert.True(e.Enforce(context, "bob", "domain1", "data1", "read"));
        Assert.False(e.Enforce(context, "bob", "domain1", "data1", "write"));

        // Use r3 p3 and m3 type
        context = e.CreateContext
        (
            PermConstants.RequestType3,
            PermConstants.PolicyType3,
            PermConstants.DefaultPolicyEffectType,
            PermConstants.MatcherType3
        );

        Assert.True(e.Enforce(context, new { Age = 30 }, "data2", "read"));
        Assert.False(e.Enforce(context, new { Age = 70 }, "data2", "read"));
    }

    [Fact]
    public void TestAbacComment()
    {
        var model = TestModelFixture.GetNewTestModel(TestModelFixture.AbacCommentText);
        Assert.Equal(3, model.Sections.GetRequestAssertion("r").Tokens.Count);
        Assert.Equal(2, model.Sections.GetRequestAssertion("r").Tokens["act"]);
        Assert.Equal(3, model.Sections.GetPolicyAssertion("p").Tokens.Count);
        Assert.Equal("some(where (p.eft == allow))", model.Sections.GetPolicyEffectAssertion("e").Value);
        Assert.Equal("r.sub == p.sub && r.obj == p.obj && r.act == p.act",
            model.Sections.GetMatcherAssertion("m").Value);
    }

    [Fact]
    public void TestRbacComment()
    {
        var model = TestModelFixture.GetNewTestModel(TestModelFixture.RbacCommentText);
        Assert.Equal(3, model.Sections.GetRequestAssertion("r").Tokens.Count);
        Assert.Equal(2, model.Sections.GetRequestAssertion("r").Tokens["act"]);
        Assert.Equal(3, model.Sections.GetPolicyAssertion("p").Tokens.Count);
        Assert.Equal("_, _", model.Sections.GetRoleAssertion("g").Value);
        Assert.Equal("some(where (p.eft == allow))", model.Sections.GetPolicyEffectAssertion("e").Value);
        Assert.Equal("g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act",
            model.Sections.GetMatcherAssertion("m").Value);
    }

    [Fact]
    public void TestModelWithCommaAndQuotations()
    {
        Enforcer e = new Enforcer(TestModelFixture.GetNewCommaAndQuotationsModel());

        TestEnforce(e, "alice", "Comma,Test", "Get", true);
        TestEnforce(e, "alice", "Comma,Test", "Post", false);
        TestEnforce(e, "alice", "\"Comma,Test\"", "Get", false);
        TestEnforce(e, "bob", "\"Comma\",\"Quotations\",Test", "Get", true);
        TestEnforce(e, "bob", "\"Comma\",\"Quotations\",Test", "Post", false);
        TestEnforce(e, "bob", "\"\"Comma\"\",\"\"Quotations\"\",Test", "Get", false);
        TestEnforce(e, "bob", "\"\"\"Comma\"\",\"\"Quotations\"\",Test\"", "Get", false);
        TestEnforce(e, "cindy", "\"Muti Quotations Test", "Get", true);
        TestEnforce(e, "cindy", "\"Muti Quotations Test", "Post", false);
        TestEnforce(e, "cindy", "\"\"Muti Quotations Test", "Get", false);
        TestEnforce(e, "cindy", "\"\"Muti Quotations Test\"", "Get", false);
    }

    [Fact]
    public void TestModelWithTabs()
    {
        Enforcer e = new Enforcer(TestModelFixture.GetNewTabsModel());
        e.AddRoleForUserInDomain("/user/john", "admin", "/tenant/1");
        Assert.True(e.Enforce("/user/john", "/tenant/1", "/tenant/1/resource", "Write"));
    }

    [Fact]
    public void TestRbacTokensWithSubstringRelation()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacTokensWithSubstringRelationModelText,
            TestModelFixture.RbacTokensWithSubstringRelationPolicyText));
        e.BuildRoleLinks();

        TestDomainEnforce(e, "alice", "tenant1", "data1", "read", true);
        TestDomainEnforce(e, "alice", "tenant1", "freeread", "read", true);
        TestDomainEnforce(e, "alice", "tenant2", "data2", "read", false);
        TestDomainEnforce(e, "alice", "tenant1", "data1", "write", false);
        TestDomainEnforce(e, "bob", "tenant1", "data1", "read", false);
        TestDomainEnforce(e, "alice", "tenant3", "freeread", "read", false);
        TestDomainEnforce(e, "alice", "tenant1", "freeread", "write", false);

    }

    [Fact]
    public void TestAbacTokensWithSubstringRelation()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.AbacTokensWithSubstringRelationModelText,
            TestModelFixture.AbacTokensWithSubstringRelationPolicyText));

        TestResource data1 = new("data1", "alice");
        TestResource data2 = new("data2", "bob");
        TestSubject subjecta = new("alice", 16);
        TestSubject subjectb = new("bob", 65);
        TestSubject subjectc = new("candy", 30);
        TestSubject subjectd = new("donale", -1);
        TestSubject subjecte = new("eleena", 1000000009);

        TestEnforce(e, subjecta, data1, "read", true);
        TestEnforce(e, subjectb, data2, "write", true);
        TestEnforce(e, subjectc, data1, "read", true);
        TestEnforce(e, subjectc, data2, "write", true);
        TestEnforce(e, subjecta, data2, "write", true);
        TestEnforce(e, subjectb, data1, "read", true);

        TestEnforce(e, subjecta, data1, "write", true);
        TestEnforce(e, subjectb, data2, "read", true);

        TestEnforce(e, subjectc, data1, "write", false);
        TestEnforce(e, subjectc, data2, "read", false);
        TestEnforce(e, subjectd, data1, "read", false);
        TestEnforce(e, subjecte, data2, "write", false);
    }

    [Fact]
    public void TestBackslashLineFeed()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.BackslashLineFeedModelText,
            TestModelFixture.BackslashLineFeedPolicyText));

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestAccidentalCacheRead()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "aliced", "ata1", "read", false);
        TestEnforce(e, "alice", "data", "1read", false);
    }

    [Fact]
    public void TestRbacWithIndexMatcher()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithIndexMatcherModelText,
            TestModelFixture.RbacWithIndexMatcherPolicyText));
        e.BuildRoleLinks();
        var rule = new Dictionary<string, Dictionary<string, string>>
        {
            ["CompanyData"] = new() { ["CompanyIsActive"] = "True", ["BusinessRole"] = "Role1" }
        };
        Assert.True(e.Enforce(rule, "WebApp", "/api/transactions/getTransactions", "POST"));
        Assert.False(e.Enforce(rule, "Admin", "/api/transactions/getTransactions", "POST"));

        rule = new Dictionary<string, Dictionary<string, string>>
        {
            ["CompanyData"] = new() { ["CompanyIsActive"] = "False", ["BusinessRole"] = "Role1" }
        };
        Assert.False(e.Enforce(rule, "WebApp", "/api/transactions/getTransactions", "POST"));
        Assert.True(e.Enforce(rule, "Admin", "/api/transactions/getTransactions", "POST"));
    }

    [Fact]
    public void TestAbacWithDynamicValueType()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.AbacWithDynamicValueTypeModelText,
            TestModelFixture.AbacWithDynamicValueTypePolicyText));
        var sub = new { Name = "bob" };
        var obj1 = new { Object = "/data1", Property1 = "prop-1" };
        var obj2 = new { Object = "/data2", Property2 = "prop-2" };
        var obj3 = new { Object = "/data2", Property3 = "prop-3" };
        Assert.True(e.Enforce(sub, obj1, "read"));
        Assert.True(e.Enforce(sub, obj2, "read"));
        Assert.False(e.Enforce(sub, obj3, "read"));
        // Request again to test the cache hit logic.
        Assert.True(e.Enforce(sub, obj1, "read"));
        Assert.True(e.Enforce(sub, obj2, "read"));
        Assert.False(e.Enforce(sub, obj3, "read"));
    }

    public class TestResource
    {
        public TestResource(string name, string owner)
        {
            Name = name;
            Owner = owner;
        }

        public string Name { get; }

        public string Owner { get; }
    }

    public class TestSubject
    {
        public TestSubject(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; }

        public int Age { get; }
    }
}
