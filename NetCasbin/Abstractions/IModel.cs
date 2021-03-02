using System.Collections.Generic;
using Casbin.Model;

namespace Casbin
{
    public interface IModel : IPolicy
    {
        public Dictionary<string, Dictionary<string, Assertion>> Sections { get; }

        public void LoadModelFromFile(string path);

        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);
    }
}
