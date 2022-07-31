using Casbin.Persist;

namespace Casbin.Model.Holder;

public class AdapterHolder
{
    private IReadOnlyAdapter _adapter;

    public AdapterHolder()
    {
    }

    public AdapterHolder(IReadOnlyAdapter adapter)
    {
        _adapter = adapter;
        DetermineAdapter(adapter);
    }

    public IReadOnlyAdapter Adapter
    {
        get => _adapter;
        set
        {
            _adapter = value;
            DetermineAdapter(value);
        }
    }

    public IBatchAdapter BatchAdapter { get; private set; }

    public IEpochAdapter EpochAdapter { get; private set; }

    public IFilteredAdapter FilteredAdapter { get; private set; }

    public ISingleAdapter SingleAdapter { get; private set; }

    private void DetermineAdapter(IReadOnlyAdapter adapter)
    {
        SingleAdapter = adapter as ISingleAdapter;
        BatchAdapter = adapter as IBatchAdapter;
        EpochAdapter = adapter as IEpochAdapter;
        FilteredAdapter = adapter as IFilteredAdapter;
    }
}
