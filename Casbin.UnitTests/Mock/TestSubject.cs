namespace Casbin.UnitTests.Mock;

public class TestSubject
{
    public TestSubject(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name { get; }

    public int Age { get; }
}
