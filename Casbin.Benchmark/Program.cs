using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Casbin.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Uncomment this line if you want to debug the benchmarks
            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
