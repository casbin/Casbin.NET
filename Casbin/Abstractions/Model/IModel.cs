using Casbin.Caching;
using Casbin.Evaluation;
using Casbin.Model.Holder;

namespace Casbin.Model
{
    public interface IModel
    {
        public string Path { get; }
        public ISections Sections { get; }

        public PolicyStoreHolder PolicyStoreHolder { get; }
        public AdapterHolder AdapterHolder { get; }
        public EffectorHolder EffectorHolder { get; }
        public WatcherHolder WatcherHolder { get; }

        public IExpressionHandler ExpressionHandler { get; set; }
        public IEnforceViewCache EnforceViewCache { get; set; }
        public IEnforceCache EnforceCache { get; set; }
        public IGFunctionCachePool GFunctionCachePool { get; set; }

        public void LoadModelFromFile(string path);
        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);
    }
}
