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
    }
}
