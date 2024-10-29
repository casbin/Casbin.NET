using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist.Adapter.Stream;

public class StreamAdapter : BaseAdapter, IEpochAdapter, IFilteredAdapter
{
    public StreamAdapter(System.IO.Stream stream)
    {
        SetLoadFromStream(stream);
    }
}
