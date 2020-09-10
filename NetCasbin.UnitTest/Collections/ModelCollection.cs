using NetCasbin.UnitTest.Fixtures;
using Xunit;

namespace NetCasbin.UnitTest.Collections
{
    [CollectionDefinition("Model collection")]
    public class ModelCollection : ICollectionFixture<TestModelFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
