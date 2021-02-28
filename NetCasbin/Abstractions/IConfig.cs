namespace Casbin
{
    public interface IConfig
    {
        public string Get(string key);

        public bool GetBool(string key);

        public int GetInt(string key);

        public float GetFloat(string key);

        public string GetString(string key);

        public string[] GetStrings(string key);

        public void Set(string key, string value);
    }
}
