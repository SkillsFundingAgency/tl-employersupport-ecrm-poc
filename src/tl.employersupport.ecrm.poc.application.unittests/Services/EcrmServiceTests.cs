using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace tl.employersupport.ecrm.poc.application.unittests.Services
{
    public class EcrmServiceTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper _output;

        public EcrmServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EcrmService_Constructor_Succeeds_With_Valid_Parameters()
        {
            var _ = new EcrmServiceBuilder().Build();
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void EcrmService_Constructor_Guards_Against_NullParameters()
        {
            typeof(EcrmService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void EcrmService_Constructor_Guards_Against_BadParameters()
        {
            typeof(EcrmService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task EcrmService_FindEmployer_Returns_Expected_Value()
        {
            var employer = new EmployerBuilder()
                .Build();

            var apiClient = Substitute.For<IEcrmApiClient>();
            apiClient.GetEmployer(Arg.Any<EmployerSearchRequest>())
                .Returns(employer);

            var service = new EcrmServiceBuilder().Build(apiClient);

            var searchRequest = new EmployerSearchRequestBuilder()
                .Build();

            var searchResult = await service.FindEmployer(searchRequest);

            searchResult.Should().NotBeNull();
            searchResult.Should().Be(employer);
        }

        [Fact]
        public async Task EcrmService_Heartbeat_Returns_Expected_Value()
        {
            var apiClient = Substitute.For<IEcrmApiClient>();
            apiClient.GetHeartbeat()
                .Returns(true);

            var service = new EcrmServiceBuilder().Build(apiClient);

            var result = await service.Heartbeat();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task EcrmService_WhoAmI_Returns_Expected_Value()
        {
            var apiClient = Substitute.For<IEcrmODataApiClient>();

            var whoIAm = new WhoAmIResponse
            {
                BusinessUnitId = Guid.Parse("eed747ad-6aba-4af9-b92a-9843e506c28e"),
                UserId = Guid.Parse("92f7f5e5-dfb6-4833-8b21-da22e1ecdb1a"),
                OrganizationId = Guid.Parse("3c8902ef-83cc-4d22-a083-0d0f1e8fcf83")
            };

            apiClient.GetWhoAmI()
                .Returns(whoIAm);

            var service = new EcrmServiceBuilder().Build(ecrmODataApiClient: apiClient);

            var result = await service.WhoAmI();

            result.Should().NotBeNull();
            result.BusinessUnitId.Should().Be(whoIAm.BusinessUnitId);
            result.UserId.Should().Be(whoIAm.UserId);
            result.OrganizationId.Should().Be(whoIAm.OrganizationId);
        }
    }
}
