namespace Casbin.Benchmark
{
    public class TestResource
    {
        public TestResource(string name, string owner)
        {
            this.name = name;
            this.owner = owner;
        }

#pragma warning disable IDE1006 // 命名样式
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public string name { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public string owner { get; set; }
#pragma warning restore IDE1006 // 命名样式
    }
}
