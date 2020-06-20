namespace NetCasbin.Model
{
    public class PermConstants
    {
        public const string DefautRequestType = "r";

        public const string DefautPolicyType = "p";
        public const string PolicyType2 = "p2";
        public const string PolicyType3 = "p2";

        public const string DefaultRoleType = "g";
        public const string RoleType2 = "g2";
        public const string RoleType3 = "g3";

        public const string DefaultGroupingPolicyType = "g";
        public const string GroupingPolicyType2 = "g2";
        public const string GroupingPolicyType3 = "g3";

        public const string DefaultMatcherType = "m";
        public const string DefaultPolicyEffeftType = "e";

        public static class Section
        {
            // section |     sectionName
            //   "r"   | "request_definition"
            //   "p"   | "policy_definition"
            //   "g"   |  "role_definition"
            //   "e"   |   "policy_effect"
            //   "m"   |     "matchers"

            public const string RequestSection = "r";
            public const string RequestSectionName = "request_definition";

            public const string PolicySection = "p";
            public const string PolicySectionName = "policy_definition";

            public const string RoleSection = "g";
            public const string RoleSectionName = "role_definition";

            public const string PolicyEffeftSection = "e";
            public const string PolicyEffeftSectionName = "policy_effect";

            public const string MatcherSection = "m";
            public const string MatcherSectionName = "matchers";
        }

        public static class PolicyEffeft
        {
            public const string AllowOverride = "some(where (p.eft == allow))";
            public const string DenyOverride = "!some(where (p.eft == deny))";
            public const string AllowAndDeny = "some(where (p.eft == allow)) && !some(where (p.eft == deny))";
            public const string Priority = "priority(p.eft) || deny";
        }
    }
}
