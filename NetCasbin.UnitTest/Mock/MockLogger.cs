#if !NET452
using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace NetCasbin.UnitTest.Mock
{
    public class MockLogger<T> : ILogger<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MockLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string outPut = formatter(state, null);
            _testOutputHelper.WriteLine(outPut);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
#endif
