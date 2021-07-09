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
        public async Task EcrmService_CreateAccount_Returns_Expected_Value()
        {
            var apiClient = Substitute.For<IEcrmODataApiClient>();

            var accountId = Guid.Parse("63b6baf5-3f74-4ac3-8e9a-3a7afad9ad4d");

            var account = new EcrmAccountBuilder().Build(accountId);

            apiClient.CreateAccount(account)
                .Returns(accountId);

            var service = new EcrmServiceBuilder().Build(ecrmODataApiClient: apiClient);

            var result = await service.CreateAccount(account);

            result.Should().Be(accountId);
        }

        [Fact]
        public async Task EcrmService_CreateContact_Returns_Expected_Value()
        {
            var apiClient = Substitute.For<IEcrmODataApiClient>();

            var accountId = Guid.Parse("63b6baf5-3f74-4ac3-8e9a-3a7afad9ad4d");
            var contactId = Guid.Parse("3dd08977-43c7-473a-b565-5d1cc6540700");

            var contact = new EcrmContactBuilder().Build(null, accountId);

            apiClient.CreateContact(contact)
                .Returns(contactId);

            var service = new EcrmServiceBuilder().Build(ecrmODataApiClient: apiClient);

            var result = await service.CreateContact(contact);

            result.Should().Be(contactId);
        }

        [Fact]
        public async Task EcrmService_GetAccount_Returns_Expected_Value()
        {
            var apiClient = Substitute.For<IEcrmODataApiClient>();

            var accountId = Guid.Parse("63b6baf5-3f74-4ac3-8e9a-3a7afad9ad4d");

            var account = new EcrmAccountBuilder().Build(accountId);

            apiClient.GetAccount(accountId)
                .Returns(account);

            var service = new EcrmServiceBuilder().Build(ecrmODataApiClient: apiClient);

            var result = await service.GetAccount(accountId);

            result.Should().NotBeNull();
            result.AccountId.Should().Be(accountId);
            result.Name.Should().Be(account.Name);
        }

        [Fact]
        public async Task EcrmService_WhoAmI_Returns_Expected_Value()
        {
            var apiClient = Substitute.For<IEcrmODataApiClient>();
            var xrmClient = Substitute.For<IEcrmXrmClient>();

            var whoIAm = new WhoAmIBuilder().Build();

            apiClient.GetWhoAmI().Returns(whoIAm);
            xrmClient.WhoAmI().Returns(whoIAm);

            var service = new EcrmServiceBuilder().Build(
                ecrmODataApiClient: apiClient,
                ecrmXrmClient: xrmClient);

            var result = await service.WhoAmI();

            result.Should().NotBeNull();
            result.BusinessUnitId.Should().Be(whoIAm.BusinessUnitId);
            result.UserId.Should().Be(whoIAm.UserId);
            result.OrganizationId.Should().Be(whoIAm.OrganizationId);
        }
    }
}
