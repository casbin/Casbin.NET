using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Casbin.UnitTests.ParallelTest
{
    using RbacRequest = RequestValues<string, string, string, string>;

    [Collection("Model collection")]
    public class SyncParallelTest
    {
        private readonly ITestOutputHelper _output;
        private readonly TestModelFixture _testModelFixture;
        private RbacParallelTestHelper<RbacRequest> _rbacParallelTestHelper;

        public SyncParallelTest(ITestOutputHelper output, TestModelFixture testModelFixture)
        {
            _output = output;
            _testModelFixture = testModelFixture;
        }

        private void InitRbacParallelTestHelper()
        {
            Enforcer e1 = new(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e1.BuildRoleLinks();
            Enforcer e2 = new(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e2.BuildRoleLinks();
            Enforcer e3 = new(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e3.BuildRoleLinks();
            var consumer = new DefaultRbacConsumer<RbacRequest>(e1);
            var transactionFactory = new DefaultTransactionFactory();
            RandomRequestGenerator<RbacRequest> rdg = new RandomRequestGenerator<RbacRequest>(
                DefaultExistedEntropyPool.Create4(e2),
                DefaultRandomEntropyPool.Create4(
                    new KeyValuePair<string, int>("sub", 10),
                    new KeyValuePair<string, int>("dom", 10),
                    new KeyValuePair<string, int>("obj", 10),
                    new KeyValuePair<string, int>("act", 10)
                ),
                new PureExistedEntropyHandler<RbacRequest>(50),
                new PureRandomEntropyHandler<RbacRequest>(30)
            );
            _rbacParallelTestHelper = new RbacParallelTestHelper<RbacRequest>(
                consumer, e3, transactionFactory, rdg
            );
        }

        [Fact]
        public async Task RbacWithDomainsSyncParallelTestAsync()
        {
            InitRbacParallelTestHelper();
            await _rbacParallelTestHelper.TestCorrectness(2000, 1000, 0, 1000);
        }
    }
}
