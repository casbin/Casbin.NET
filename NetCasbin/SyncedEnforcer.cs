using System.IO;
using Casbin.Adapter.File;
using Casbin.Extensions;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class SyncedEnforcer
    {
        public static IEnforcer Create(IAdapter adapter = null)
        {
            return new Enforcer(Model.SyncedModelExtension.Create(), adapter);
        }

        public static IEnforcer Create(string modelPath, string policyPath)
        {
            return Create(modelPath, new FileAdapter(policyPath));
        }

        public static IEnforcer Create(string modelPath, IAdapter adapter = null)
        {
            IModel model = DefaultModel.CreateFromFile(modelPath);
            return Create(model, adapter);
        }

        public static IEnforcer Create(IModel model, IAdapter adapter = null)
        {
            model = model.ReplacePolicyManager(ReaderWriterPolicyManager.Create());
            return DefaultEnforcer.Create(model, adapter);
        }
    }
}
