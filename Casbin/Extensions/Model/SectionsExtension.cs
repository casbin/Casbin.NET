#nullable enable
using System.Collections.Generic;

namespace Casbin.Model;

public static class SectionsExtension
{
    public static string GetValue(this ISections sections, string section, string type) =>
        sections.GetAssertion<Assertion>(section, type).Value;

    public static RequestAssertion? GetRequestAssertion(this ISections sections, string type) =>
        sections.GetRequestAssertion(PermConstants.Section.RequestSection, type);

    public static RequestAssertion? GetRequestAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<RequestAssertion>(section, type);

    public static PolicyAssertion GetPolicyAssertion(this ISections sections, string type) =>
        sections.GetPolicyAssertion(PermConstants.Section.PolicySection, type);

    public static PolicyAssertion GetPolicyAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<PolicyAssertion>(section, type);

    public static IDictionary<string, PolicyAssertion> GetPolicyAssertions(this ISections sections) =>
        sections.GetPolicyAssertions(PermConstants.Section.PolicySection);

    public static IDictionary<string, PolicyAssertion> GetPolicyAssertions(this ISections sections, string section) =>
        sections.GetAssertions<PolicyAssertion>(section);

    public static RoleAssertion GetRoleAssertion(this ISections sections, string type) =>
        sections.GetRoleAssertion(PermConstants.Section.RoleSection, type);

    public static RoleAssertion GetRoleAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<RoleAssertion>(section, type);

    public static IDictionary<string, RoleAssertion> GetRoleAssertions(this ISections sections) =>
        sections.GetRoleAssertions(PermConstants.Section.RoleSection);

    public static IDictionary<string, RoleAssertion> GetRoleAssertions(this ISections sections, string section) =>
        sections.GetAssertions<RoleAssertion>(section);

    public static PolicyEffectAssertion GetPolicyEffectAssertion(this ISections sections, string type) =>
        sections.GetPolicyEffectAssertion(PermConstants.Section.PolicyEffectSection, type);

    public static PolicyEffectAssertion
        GetPolicyEffectAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<PolicyEffectAssertion>(section, type);

    public static MatcherAssertion GetMatcherAssertion(this ISections sections, string type) =>
        sections.GetMatcherAssertion(PermConstants.Section.MatcherSection, type);

    public static MatcherAssertion GetMatcherAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<MatcherAssertion>(section, type);
}
