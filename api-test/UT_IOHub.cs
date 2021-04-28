using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.SignalR;
using api.Hubs;
using System.Threading;
using System.Threading.Tasks;
using SignalR_UnitTestingSupport.Hubs;

namespace api_test
{
    [TestFixture]
    public class IOHub_Tests : HubUnitTestsBase
    {
        
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task Test_ReceiveOutput()
        {
            IOHub hub = new IOHub();
            AssignToHubRequiredProperties(hub);

            // act
            await hub.SendContent("u", "t");

            // assert
            ClientsAllMock
                .Verify(
                    x => x.SendCoreAsync(
                        "ReceiveOutput",
                        new object[] { "u", "t" },
                        It.IsAny<CancellationToken>())
                );
        }
    }
}