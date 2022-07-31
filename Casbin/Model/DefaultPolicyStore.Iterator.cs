namespace Casbin.Model;

public partial class DefaultPolicyStore
{
    internal ref struct Iterator
    {
        private int _index;
        private readonly int _size;
        private readonly Node _node;

        internal Iterator(Node node)
        {
            _node = node;
            _index = 0;
            _size = node.Policy.Count;
        }

        public bool HasNext() => _index < _size;

        public bool GetNext(out IPolicyValues values)
        {
            if (_node is null)
            {
                values = default;
                return false;
            }

            if (_index is 0 && _node.Lock.IsReadLockHeld is false)
            {
                _node.Lock.EnterReadLock();
            }

            if (_index < _size)
            {
                values = _node.Policy[_index++];
                return true;
            }

            if (_node.Lock.IsReadLockHeld)
            {
                _node.Lock.ExitReadLock();
            }

            values = default;
            return false;
        }

        public void Interrupt()
        {
            if (_node.Lock.IsReadLockHeld)
            {
                _node.Lock.ExitReadLock();
            }
        }
    }
}
