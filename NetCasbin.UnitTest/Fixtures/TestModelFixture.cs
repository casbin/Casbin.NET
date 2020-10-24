using System;
using System.IO;
using System.Text;
using NetCasbin.Persist.FileAdapter;

namespace NetCasbin.UnitTest.Fixtures
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
        internal readonly string _priorityModelText = ReadTestFile("priority_model.conf");
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
        internal readonly string _priorityIndeterminatePolicyText = ReadTestFile("priority_indeterminate_policy.csv");
        internal readonly string _rbacPolicyText = ReadTestFile("rbac_policy.csv");
        internal readonly string _rbacWithDenyPolicyText = ReadTestFile("rbac_with_deny_policy.csv");
        internal readonly string _rbacWithDomainsPolicyText = ReadTestFile("rbac_with_domains_policy.csv");
        internal readonly string _rbacWithHierarchyPolicyText = ReadTestFile("rbac_with_hierarchy_policy.csv");
        internal readonly string _rbacWithResourceRolePolicyText = ReadTestFile("rbac_with_resource_roles_policy.csv");

        public Model.Model GetNewAbacModel()
        {
            return GetNewTestModel(_abacModelText);
        }

        public Model.Model GetNewAbacWithEvalModel()
        {
            return GetNewTestModel(_abacWithEvalModelText, _abacWithEvalPolicyText);
        }

        public Model.Model GetBasicTestModel()
        {
            return GetNewTestModel(_basicModelText, _basicPolicyText);
        }

        public Model.Model GetBasicWithoutResourceTestModel()
        {
            return GetNewTestModel(_basicWithoutResourceModelText, _basicWithoutResourcePolicyText);
        }

        public Model.Model GetBasicWithoutUserTestModel()
        {
            return GetNewTestModel(_basicWithoutUserModelText, _basicWithoutUserPolicyText);
        }

        public Model.Model GetNewKeyMatchTestModel()
        {
            return GetNewTestModel(_keyMatchModelText, _keyMatchPolicyText);
        }

        public Model.Model GetNewKeyMatch2TestModel()
        {
            return GetNewTestModel(_keyMatch2ModelText, _keyMatch2PolicyText);
        }

        public Model.Model GetNewPriorityTestModel()
        {
            return GetNewTestModel(_priorityModelText, _priorityPolicyText);
        }

        public Model.Model GetNewRbacTestModel()
        {
            return GetNewTestModel(_rbacModelText, _rbacPolicyText);
        }

        public Model.Model GetNewRbacWithDenyTestModel()
        {
            return GetNewTestModel(_rbacWithDenyModelText, _rbacWithDenyPolicyText);
        }

        public Model.Model GetNewRbacWithDomainsTestModel()
        {
            return GetNewTestModel(_rbacWithDomainsModelText, _rbacWithDomainsPolicyText);
        }

        public Model.Model GetNewRbacWithResourceRoleTestModel()
        {
            return GetNewTestModel(_rbacWithResourceRoleModelText, _rbacWithResourceRolePolicyText);
        }

        public static Model.Model GetNewTestModel(string modelText)
        {
            return Model.Model.CreateDefaultFromText(modelText);
        }

        public static Model.Model GetNewTestModel(string modelText, string policyText)
        {
            return LoadModelFromMemory(GetNewTestModel(modelText), policyText);
        }

        public static string GetTestFile(string fileName)
        {
            return Path.Combine("examples", fileName);
        }

        private static Model.Model LoadModelFromMemory(Model.Model model, string policy)
        {
            model.ClearPolicy();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(policy)))
            {
                DefaultFileAdapter fileAdapter = new DefaultFileAdapter(ms);
                fileAdapter.LoadPolicy(model);
            }
            model.RefreshPolicyStringSet();
            return model;
        }

        private static string ReadTestFile(string fileName)
        {
            return File.ReadAllText(GetTestFile(fileName));
        }
    }
}
