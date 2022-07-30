namespace Casbin.Model;

public interface IRequestValues
{
    public string this[int index] { get; }

    public int Count { get; }

    public bool TrySetValue<T>(int index, T value);
}
