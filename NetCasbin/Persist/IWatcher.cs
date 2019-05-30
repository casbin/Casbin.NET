using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin.Persist
{
    public interface IWatcher
    {
        void SetUpdateCallback();

        void Update();
    }
}
