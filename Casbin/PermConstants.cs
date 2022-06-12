namespace Casbin
{
    public static class PermConstants
    {
        public const char PolicySeparatorChar = ',';
        public const string PolicySeparatorString = ", "; // include a white space
        public const string SubjectPrioritySeparatorString = "::";

        public const string DefaultRequestType = "r";
        public const string RequestType2 = "r2";
        public const string RequestType3 = "r3";

        public const string DefaultPolicyType = "p";
        public const string PolicyType2 = "p2";
        public const string PolicyType3 = "p3";

        public const string DefaultRoleType = "g";
        public const string RoleType2 = "g2";
        public const string RoleType3 = "g3";

        public const string DefaultGroupingPolicyType = DefaultRoleType;
        public const string GroupingPolicyType2 = RoleType2;
        public const string GroupingPolicyType3 = RoleType3;

        public const string DefaultMatcherType = "m";
        public const string MatcherType2 = "m2";
        public const string MatcherType3 = "m3";

        public const string DefaultPolicyEffectType = "e";
        public const string PolicyEffectType2 = "e2";
        public const string PolicyEffectType3 = "e3";

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

            public const string PolicyEffectSection = "e";
            public const string PolicyEffectSectionName = "policy_effect";

            public const string MatcherSection = "m";
            public const string MatcherSectionName = "matchers";
        }

        public static class PolicyEffect
        {
            public const string AllowOverride = "some(where (p.eft == allow))";
            public const string DenyOverride = "!some(where (p.eft == deny))";
            public const string AllowAndDeny = "some(where (p.eft == allow)) && !some(where (p.eft == deny))";
            public const string Priority = "priority(p.eft) || deny";
            public const string SubjectPriority = "subjectPriority(p.eft) || deny";
            public const string PriorityDenyOverride = "priority(p.eft) && !some(where (p.eft == deny))";
        }
    }
}
