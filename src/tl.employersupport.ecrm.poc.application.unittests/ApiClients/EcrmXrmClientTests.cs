using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.ApiClients
{
    public class EcrmXrmClientTests
    {
        [Fact]
        public void EcrmODataApiClient_Constructor_Succeeds_With_Valid_Parameters()
        {
            var _ = new EcrmXrmClientBuilder().Build();
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void EcrmXrmClient_Constructor_Guards_Against_NullParameters()
        {
            typeof(EcrmXrmClient)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void EcrmXrmClient_Constructor_Guards_Against_BadParameters()
        {
            typeof(EcrmXrmClient)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        //[Fact]
        public async Task EcrmXrmClient_GetWhoAmI_Returns_Expected_Value()
        {
            //Hard to mock
            //https://dynamicsvalue.com/blog/fake-xrm-easy-versus-other-frameworks
            //Could use reflection - https://hermens.com.au/2017/01/24/Unit-Testing-Sealed-Internal-Classes/
            //Could use wrapper - https://alexanderdevelopment.net/post/2013/01/12/how-to-unit-test-c-dynamics-crm-interface-code-part-iii/
            //var whoAmIResponse = new WhoAmIResponseBuilder().Build();
            //var whoAmIResponse = new WhoAmIResponseBuilder().Build();

            var organizationService = Substitute.For<IOrganizationService>();
            //organizationService.Execute(Arg.Any<WhoAmIRequest>())
            //    .Returns(whoAmIResponse);

            var client = new EcrmXrmClientBuilder().Build();

            var whoAmI = await client.WhoAmI();

            whoAmI.Should().NotBeNull();
            //whoAmI.BusinessUnitId.Should().Be(whoAmIResponse.BusinessUnitId);
            //whoAmI.OrganizationId.Should().Be(whoAmIResponse.OrganizationId);
            //whoAmI.UserId.Should().Be(whoAmIResponse.UserId);
        }
    }
}
