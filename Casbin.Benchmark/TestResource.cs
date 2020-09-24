namespace Casbin.Benchmark
{
    public class TestResource
    {
        public TestResource(string name, string owner)
        {
            Name = name;
            Owner = owner;
        }

        public string Name { get; set; }

        public string Owner { get; set; }
    }
}
