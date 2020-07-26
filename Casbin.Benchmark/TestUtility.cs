using System.IO;

namespace Casbin.Benchmark
{
    public static class TestHelper
    {
        public static string GetTestFilePath(string fileName)
        {
            return Path.Combine("examples", fileName);
        }
    }
}
