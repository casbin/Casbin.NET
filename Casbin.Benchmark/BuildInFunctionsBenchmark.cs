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
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net50)]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net60)]
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
        public void KeyMatch4(string arg1, string arg2)
        {
            _ = BuiltInFunctions.KeyMatch4(arg1, arg2);
        }

        public IEnumerable<object[]> IPMatchTestData() => new[]
        {
            new object[] {"192.168.2.123", "192.168.2.123"},
            new object[] {"192.168.2.123", "192.168.2.0/24"}
        };

        [Benchmark]
        [BenchmarkCategory(nameof(IPMatch))]
        [ArgumentsSource(nameof(IPMatchTestData))]
        public void IPMatch(string arg1, string arg2)
        {
            _ = BuiltInFunctions.IPMatch(arg1, arg2);
        }
    }
}
