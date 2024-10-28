using System;
using System.Reflection;

namespace Casbin.Persist.Adapter.File;

public class FileAdapter : BaseAdapter, IEpochAdapter, IFilteredAdapter
{
    public FileAdapter(string filePath)
    {
        SetLoadFromPath(filePath);
    }

    [Obsolete("Please use StreamAdapter instead")]
    public FileAdapter(System.IO.Stream originalStream)
    {
        SetLoadFromStream(originalStream);
    }
}
