using Casbin.Caching;
using Casbin.Evaluation;

namespace Casbin.Model
{
    public interface IModel : IPolicy
    {
        public bool IsSynchronized { get; }

        public string ModelPath { get; }

        public IEnforceViewCache EnforceViewCache { get; }

        public IExpressionHandler ExpressionHandler { get; }

        public IPolicyManager PolicyManager { get; set; }

        public void LoadModelFromFile(string path);

        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);
    }
}
