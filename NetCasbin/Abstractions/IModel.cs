namespace Casbin
{
    public interface IModel : IPolicy
    {
        public IPolicyManager PolicyManager { get; }

        public void SetPolicyManager(IPolicyManager policyManager);

        public void LoadModelFromFile(string path);

        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);
    }
}
