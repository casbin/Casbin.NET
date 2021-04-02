using System.Threading;
using Casbin.Persist;

namespace Casbin.Model
{
    public class ReaderWriterPolicyManager : DefaultPolicyManager
    {
        private readonly ReaderWriterPolicyManagerOptions _options;
        private readonly ReaderWriterLockSlim _lockSlim = new();

        public ReaderWriterPolicyManager(IPolicy policy, IAdapter adapter = null)
            : base(policy, adapter)
        {
            _options = new ReaderWriterPolicyManagerOptions();
        }

        public ReaderWriterPolicyManager(IPolicy policy, ReaderWriterPolicyManagerOptions options, IAdapter adapter = null)
            : base(policy, adapter)
        {
            _options = options;
        }

        public static new IPolicyManager Create()
        {
            return new ReaderWriterPolicyManager(DefaultPolicy.Create());
        }

        public override bool IsSynchronized => true;

        public override void StartRead()
        {
            _lockSlim.EnterReadLock();
        }

        public override void StartWrite()
        {
            _lockSlim.EnterWriteLock();
        }

        public override void EndRead()
        {
            _lockSlim.ExitReadLock();
        }

        public override void EndWrite()
        {
            _lockSlim.ExitWriteLock();
        }

        public override bool TryStartRead()
        {
            return _lockSlim.TryEnterReadLock(_options.WaitTimeOut);
        }

        public override bool TryStartWrite()
        {
            return _lockSlim.TryEnterWriteLock(_options.WaitTimeOut);
        }
    }
}
