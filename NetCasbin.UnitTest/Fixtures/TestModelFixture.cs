using System.IO;
using System.Text;
using Casbin.Adapter.File;
using Casbin.Model;

namespace Casbin.UnitTests.Fixtures
{
    public class TestModelFixture
    {
        internal readonly string _abacModelText = ReadTestFile("abac_model.conf");
        internal readonly string _abacWithEvalModelText = ReadTestFile("abac_rule_model.conf");
        internal readonly string _basicModelText = ReadTestFile("basic_model.conf");
        internal readonly string _basicWithRootModelText = ReadTestFile("basic_with_root_model.conf");
        internal readonly string _basicWithoutResourceModelText = ReadTestFile("basic_without_resources_model.conf");
        internal readonly string _basicWithoutUserModelText = ReadTestFile("basic_without_users_model.conf");
        internal readonly string _ipMatchModelText = ReadTestFile("ipmatch_model.conf");
        internal readonly string _keyMatchModelText = ReadTestFile("keymatch_model.conf");
        internal readonly string _keyMatch2ModelText = ReadTestFile("keymatch2_model.conf");
        internal readonly string _keyMatchCustomModelText = ReadTestFile("keymatch_custom_model.conf");
        internal readonly string _priorityModelText = ReadTestFile("priority_model.conf");
        internal readonly string _priorityExplicitModelText = ReadTestFile("priority_explicit_model.conf");
        internal readonly string _rbacModelText = ReadTestFile("rbac_model.conf");
        internal readonly string _rbacWithDenyModelText = ReadTestFile("rbac_with_deny_model.conf");
        internal readonly string _rbacWithNotDenyModelText = ReadTestFile("rbac_with_not_deny_model.conf");
        internal readonly string _rbacWithDomainsModelText = ReadTestFile("rbac_with_domains_model.conf");
        internal readonly string _rbacWithResourceRoleModelText = ReadTestFile("rbac_with_resource_roles_model.conf");

        internal readonly string _abacWithEvalPolicyText = ReadTestFile("abac_rule_policy.csv");
        internal readonly string _basicPolicyText = ReadTestFile("basic_policy.csv");
        internal readonly string _basicWithoutResourcePolicyText = ReadTestFile("basic_without_resources_policy.csv");
        internal readonly string _basicWithoutUserPolicyText = ReadTestFile("basic_without_users_policy.csv");
        internal readonly string _ipMatchPolicyText = ReadTestFile("ipmatch_policy.csv");
        internal readonly string _keyMatchPolicyText = ReadTestFile("keymatch_policy.csv");
        internal readonly string _keyMatch2PolicyText = ReadTestFile("keymatch2_policy.csv");
        internal readonly string _priorityPolicyText = ReadTestFile("priority_policy.csv");
        internal readonly string _priorityExplicitPolicyText = ReadTestFile("priority_explicit_policy.csv");
        internal readonly string _priorityIndeterminatePolicyText = ReadTestFile("priority_indeterminate_policy.csv");
        internal readonly string _rbacPolicyText = ReadTestFile("rbac_policy.csv");
        internal readonly string _rbacWithDenyPolicyText = ReadTestFile("rbac_with_deny_policy.csv");
        internal readonly string _rbacWithDomainsPolicyText = ReadTestFile("rbac_with_domains_policy.csv");
        internal readonly string _rbacWithDomainsPolicy2Text = ReadTestFile("rbac_with_domains_policy2.csv");
        internal readonly string _rbacWithHierarchyPolicyText = ReadTestFile("rbac_with_hierarchy_policy.csv");
        internal readonly string _rbacWithHierarchyWithDomainsPolicyText = ReadTestFile("rbac_with_hierarchy_with_domains_policy.csv");
        internal readonly string _rbacWithResourceRolePolicyText = ReadTestFile("rbac_with_resource_roles_policy.csv");

        // https://github.com/casbin/Casbin.NET/issues/154
        internal readonly string _rbacMultipleModelText = ReadTestFile("rbac_multiple_rolemanager_model.conf");
        internal readonly string _rbacMultiplePolicyText = ReadTestFile("rbac_multiple_rolemanager_policy.csv");

        // https://github.com/casbin/Casbin.NET/issues/106
        internal readonly string _rbacMultipleEvalModelText = ReadTestFile("rbac_multiple_eval_model.conf");
        internal readonly string _rbacMultipleEvalPolicyText = ReadTestFile("rbac_multiple_eval_policy.csv");

        public IModel GetNewAbacModel()
        {
            return GetNewTestModel(_abacModelText);
        }

        public IModel GetNewAbacWithEvalModel()
        {
            return GetNewTestModel(_abacWithEvalModelText, _abacWithEvalPolicyText);
        }

        public IModel GetBasicTestModel()
        {
            return GetNewTestModel(_basicModelText, _basicPolicyText);
        }

        public IModel GetBasicWithoutResourceTestModel()
        {
            return GetNewTestModel(_basicWithoutResourceModelText, _basicWithoutResourcePolicyText);
        }

        public IModel GetBasicWithoutUserTestModel()
        {
            return GetNewTestModel(_basicWithoutUserModelText, _basicWithoutUserPolicyText);
        }

        public IModel GetNewKeyMatchTestModel()
        {
            return GetNewTestModel(_keyMatchModelText, _keyMatchPolicyText);
        }

        public IModel GetNewKeyMatch2TestModel()
        {
            return GetNewTestModel(_keyMatch2ModelText, _keyMatch2PolicyText);
        }

        public IModel GetNewPriorityTestModel()
        {
            return GetNewTestModel(_priorityModelText, _priorityPolicyText);
        }

        public IModel GetNewPriorityExplicitTestModel()
        {
            return GetNewTestModel(_priorityExplicitModelText, _priorityExplicitPolicyText);
        }

        public IModel GetNewRbacTestModel()
        {
            return GetNewTestModel(_rbacModelText, _rbacPolicyText);
        }

        public IModel GetNewRbacWithDenyTestModel()
        {
            return GetNewTestModel(_rbacWithDenyModelText, _rbacWithDenyPolicyText);
        }

        public IModel GetNewRbacWithDomainsTestModel()
        {
            return GetNewTestModel(_rbacWithDomainsModelText, _rbacWithDomainsPolicyText);
        }

        public IModel GetNewRbacWithResourceRoleTestModel()
        {
            return GetNewTestModel(_rbacWithResourceRoleModelText, _rbacWithResourceRolePolicyText);
        }

        public static IModel GetNewTestModel(string modelText)
        {
            return DefaultModel.CreateFromText(modelText);
        }

        public static IModel GetNewTestModel(string modelText, string policyText)
        {
            return LoadModelFromMemory(GetNewTestModel(modelText), policyText);
        }

        public static string GetTestFile(string fileName)
        {
            return Path.Combine("examples", fileName);
        }

        private static IModel LoadModelFromMemory(IModel model, string policy)
        {
            model.ClearPolicy();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(policy)))
            {
                FileAdapter fileAdapter = new FileAdapter(ms);
                fileAdapter.LoadPolicy(model);
            }
            model.RefreshPolicyStringSet();
            model.SortPoliciesByPriority();
            return model;
        }

        private static string ReadTestFile(string fileName)
        {
            return File.ReadAllText(GetTestFile(fileName));
        }
    }
}
