using System;

namespace NetCasbin.Persist
{
    public interface IWatcher
    {
        void SetUpdateCallback(Action callback);

        void Update();
    }
}
