namespace Casbin.UnitTests.Mock;

public class TestResource
{
    public TestResource(string name, string owner)
    {
        Name = name;
        Owner = owner;
    }

    public string Name { get; }

    public string Owner { get; }
}
