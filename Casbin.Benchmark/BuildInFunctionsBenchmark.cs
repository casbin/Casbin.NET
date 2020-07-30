using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using NetCasbin.Util;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Functions")]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net48)]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.NetCoreApp31, baseline: true)]
    public class BuildInFunctionsBenchmark
    {
        public IEnumerable<object[]> KeyMatch4TestData() => new[]
        {
            new object[] {"/parent/123/child/123", "/parent/{id}/child/{id}"},
            new object[] {"/parent/123/child/123", "/parent/{id}/child/{another_id}"}
        };

        [Benchmark]
        [BenchmarkCategory(nameof(KeyMatch4))]
        [ArgumentsSource(nameof(KeyMatch4TestData))]
        public void KeyMatch4(string key1, string key2)
        {
            _ = BuiltInFunctions.KeyMatch4(key2, key2);
        }
    }
}
