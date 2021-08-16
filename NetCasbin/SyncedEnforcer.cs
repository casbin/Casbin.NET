using System.IO;
using Casbin.Adapter.File;
using Casbin.Extensions;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class SyncedEnforcer
    {
        public static IEnforcer Create(IReadOnlyAdapter adapter = null)
        {
            return new Enforcer(SyncedModel.Create(), adapter);
        }

        public static IEnforcer Create(string modelPath, string policyPath)
        {
            return Create(modelPath, new FileAdapter(policyPath));
        }

        public static IEnforcer Create(string modelPath, IReadOnlyAdapter adapter = null)
        {
            IModel model = DefaultModel.CreateFromFile(modelPath);
            return Create(model, adapter);
        }

        public static IEnforcer Create(IModel model, IReadOnlyAdapter adapter = null)
        {
            model = model.ReplacePolicyManager(ReaderWriterPolicyManager.Create());
            return DefaultEnforcer.Create(model, adapter);
        }
    }
}
