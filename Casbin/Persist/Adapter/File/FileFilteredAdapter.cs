namespace Casbin.Persist.Adapter.File;

public class FileFilteredAdapter : FileAdapter
{
    public FileFilteredAdapter(string filePath) : base(filePath)
    {
    }

    public FileFilteredAdapter(System.IO.Stream inputStream) : base(inputStream)
    {
    }
}
