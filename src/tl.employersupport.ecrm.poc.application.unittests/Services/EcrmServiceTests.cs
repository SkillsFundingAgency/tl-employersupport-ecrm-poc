using System;
using System.Collections.Generic;
using System.Linq;
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
            var xrmClient = Substitute.For<IEcrmXrmClient>();

            var accountId = Guid.Parse("63b6baf5-3f74-4ac3-8e9a-3a7afad9ad4d");
            var contactId = Guid.Parse("3dd08977-43c7-473a-b565-5d1cc6540700");

            var contact = new EcrmContactBuilder().Build(null, accountId);

            xrmClient.CreateContact(contact)
                .Returns(contactId);

            var service = new EcrmServiceBuilder().Build(ecrmXrmClient: xrmClient);

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

        [Fact]
        public async Task EcrmService_CreateNote_Returns_Expected_Value()
        {
            var xrmClient = Substitute.For<IEcrmXrmClient>();

            var accountId = Guid.Parse("ecd8b105-38be-4fcf-95da-dc7c3b8a612c");
            var contactId = Guid.Parse("51d955f9-7c19-4604-91bc-a488569ca6de");
            var noteId = Guid.Parse("b818eb40-6d2d-4e02-af05-3ed0c7862aae");

            var note = new EcrmNoteBuilder().Build(noteId, contactId, accountId);

            xrmClient.CreateNote(note)
                .Returns(noteId);

            var service = new EcrmServiceBuilder().Build(ecrmXrmClient: xrmClient);

            var result = await service.CreateNote(note);

            result.Should().Be(noteId);
        }

        [Fact]
        public async Task EcrmService_CheckForDuplicateAccount_Returns_Expected_Value()
        {
            var xrmClient = Substitute.For<IEcrmXrmClient>();

            var account = new EcrmAccountBuilder().Build();
            var duplicatesList = new EcrmAccountBuilder().BuildList();

            xrmClient.FindDuplicateAccounts(account)
                .Returns(duplicatesList);

            var service = new EcrmServiceBuilder().Build(ecrmXrmClient: xrmClient);

            var results = await service.FindDuplicateAccounts(account);

            var enumerable = results as Account[] ?? results.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Should().BeEmpty();
            enumerable.Should().BeEquivalentTo(duplicatesList);
        }

        [Fact]
        public async Task EcrmService_UpdateAccountCustomerType_Returns_Expected_Value()
        {
            var xrmClient = Substitute.For<IEcrmXrmClient>();

            var service = new EcrmServiceBuilder().Build(ecrmXrmClient: xrmClient);

            var accountId = Guid.Parse("c6d61f27-e0c5-4bc8-8788-1a5bcb4e1946");
            var newCustomerTypeCode = 200008;

            await service.UpdateAccountCustomerType(accountId, newCustomerTypeCode);

            await xrmClient
                .Received(1)
                .UpdateAccountCustomerType(accountId, newCustomerTypeCode);
        }
    }
}
