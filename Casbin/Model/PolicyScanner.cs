namespace Casbin.Model;

public ref struct PolicyScanner
{
    private DefaultPolicyStore.Iterator _iterator;
    internal PolicyScanner(DefaultPolicyStore.Iterator iterator) => _iterator = iterator;

    public bool HasNext() => _iterator.HasNext();

    public bool GetNext(out IPolicyValues outValues)
    {
        bool hasNext = _iterator.GetNext(out IPolicyValues values);
        outValues = values;
        return hasNext;
    }

    public void Interrupt() => _iterator.Interrupt();
}

public ref struct PolicyScanner<TRequest> where TRequest : IRequestValues
{
    private PolicyScanner _scanner;
    private readonly TRequest _request;

    internal PolicyScanner(in PolicyScanner scanner, in TRequest request)
    {
        _scanner = scanner;
        _request = request;
    }

    public bool HasNext() => _scanner.HasNext();

    public bool GetNext(out IPolicyValues outValues)
    {
        bool hasNext = _scanner.GetNext(out IPolicyValues values);
        outValues = values;
        return hasNext;
    }

    public void Interrupt() => _scanner.Interrupt();
}
