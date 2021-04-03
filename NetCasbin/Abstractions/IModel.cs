namespace Casbin
{
    public interface IModel : IPolicy
    {
        public bool IsSynchronized { get; }

        public string ModelPath { get; }

        public IPolicyManager PolicyManager { get; set; }

        public void LoadModelFromFile(string path);

        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);
    }
}
