namespace Casbin.Persist.Adapter.Text;

public class TextAdapter : BaseAdapter, IEpochAdapter, IFilteredAdapter
{
    public TextAdapter(string text)
    {
        SetLoadFromText(text);
    }
}
