using System;
using System.IO;
using System.Text;
using NetCasbin.Persist.FileAdapter;

namespace NetCasbin.UnitTest.Fixtures
{
    public class TestModelFixture
    {
        internal readonly Lazy<string> _abacModelText = LazyReadTestFile("abac_model.conf");
        internal readonly Lazy<string> _basicModelText = LazyReadTestFile("basic_model.conf");
        internal readonly Lazy<string> _basicWithRootModelText = LazyReadTestFile("basic_with_root_model.conf");
        internal readonly Lazy<string> _basicWithoutResourceModelText = LazyReadTestFile("basic_without_resources_model.conf");
        internal readonly Lazy<string> _basicWithoutUserModelText = LazyReadTestFile("basic_without_users_model.conf");
        internal readonly Lazy<string> _ipMatchModelText = LazyReadTestFile("ipmatch_model.conf");
        internal readonly Lazy<string> _keyMatchModelText = LazyReadTestFile("keymatch_model.conf");
        internal readonly Lazy<string> _keyMatch2ModelText = LazyReadTestFile("keymatch2_model.conf");
        internal readonly Lazy<string> _priorityModelText = LazyReadTestFile("priority_model.conf");
        internal readonly Lazy<string> _rbacModelText = LazyReadTestFile("rbac_model.conf");
        internal readonly Lazy<string> _rbacWithDenyModelText = LazyReadTestFile("rbac_with_deny_model.conf");
        internal readonly Lazy<string> _rbacWithNotDenyModelText = LazyReadTestFile("rbac_with_not_deny_model.conf");
        internal readonly Lazy<string> _rbacWithDomainsModelText = LazyReadTestFile("rbac_with_domains_model.conf");
        internal readonly Lazy<string> _rbacWithResourceRoleModelText = LazyReadTestFile("rbac_with_resource_roles_model.conf");

        internal readonly Lazy<string> _basicPolicyText = LazyReadTestFile("basic_Policy.csv");
        internal readonly Lazy<string> _basicWithoutResourcePolicyText = LazyReadTestFile("basic_without_resources_Policy.csv");
        internal readonly Lazy<string> _basicWithoutUserPolicyText = LazyReadTestFile("basic_without_users_Policy.csv");
        internal readonly Lazy<string> _ipMatchPolicyText = LazyReadTestFile("ipmatch_Policy.csv");
        internal readonly Lazy<string> _keyMatchPolicyText = LazyReadTestFile("keymatch_Policy.csv");
        internal readonly Lazy<string> _keyMatch2PolicyText = LazyReadTestFile("keymatch2_Policy.csv");
        internal readonly Lazy<string> _priorityPolicyText = LazyReadTestFile("priority_Policy.csv");
        internal readonly Lazy<string> _priorityIndeterminatePolicyText = LazyReadTestFile("priority_indeterminate_policy.csv");
        internal readonly Lazy<string> _rbacPolicyText = LazyReadTestFile("rbac_Policy.csv");
        internal readonly Lazy<string> _rbacWithDenyPolicyText = LazyReadTestFile("rbac_with_deny_Policy.csv");
        internal readonly Lazy<string> _rbacWithDomainsPolicyText = LazyReadTestFile("rbac_with_domains_Policy.csv");
        internal readonly Lazy<string> _rbacWithResourceRolePolicyText = LazyReadTestFile("rbac_with_resource_roles_Policy.csv");

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

        public Model.Model GetNewkeyMatchTestModel()
        {
            return GetNewTestModel(_keyMatchModelText, _keyMatchPolicyText);
        }

        public Model.Model GetNewkeyMatch2TestModel()
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

        public Model.Model GetNewTestModel(Lazy<string> modelText)
        {
            return CoreEnforcer.NewModel(modelText.Value);
        }

        public Model.Model GetNewTestModel(Lazy<string> modelText, Lazy<string> policyText)
        {
            var model = CoreEnforcer.NewModel(modelText.Value);
            return LoadModelFromMemory(model, policyText.Value);
        }

        private static Model.Model LoadModelFromMemory(Model.Model model, string policy)
        {
            model.ClearPolicy();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(policy)))
            {
                var fileAdapter = new DefaultFileAdapter(ms);
                fileAdapter.LoadPolicy(model);
            }
            model.RefreshPolicyStringSet();
            return model;
        }
        
        private static Lazy<string> LazyReadTestFile(string fileName)
        {
            return new Lazy<string>(() => File.ReadAllText(Path.Combine("examples", fileName)));
        }
    }
}
