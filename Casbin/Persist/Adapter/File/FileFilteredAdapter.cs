using System;

namespace Casbin.Persist.Adapter.File;

[Obsolete("Please use FileAdapter instead")]
public class FileFilteredAdapter : FileAdapter
{
    [Obsolete("Please use FileAdapter instead")]
    public FileFilteredAdapter(string filePath) : base(filePath)
    {
    }

    [Obsolete("Please use StreamAdapter instead")]
    public FileFilteredAdapter(System.IO.Stream originalStream) : base(originalStream)
    {
    }
}
