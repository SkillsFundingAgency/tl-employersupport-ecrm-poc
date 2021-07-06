using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using tl.employersupport.ecrm.poc.tests.common.HttpClient;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.ApiClients
{
    public class EcrmODataApiClientTests
    {
        private const string EcrmODataApiBaseUrl = "https://test.api.crm4.dynamics.com/api/data/v9.2/";
        private readonly Uri _ecrmODataApiBaseUri = new(EcrmODataApiBaseUrl);

        [Fact]
        public void EcrmODataApiClient_Constructor_Succeeds_With_Valid_Parameters()
        {
            var _ = new EcrmODataApiClientBuilder().Build();
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void EcrmODataApiClient_Constructor_Guards_Against_NullParameters()
        {
            typeof(EcrmODataApiClient)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void EcrmODataApiClient_Constructor_Guards_Against_BadParameters()
        {
            typeof(EcrmODataApiClient)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task EcrmODataApiClient_GetWhoAmI_Returns_Expected_Value()
        {
            var whoAmIJson = JsonBuilder.BuildEcrmWhoAmIResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _ecrmODataApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_ecrmODataApiBaseUri, "WhoAmI"), whoAmIJson
                            }
                        });

            var authenticationService = Substitute.For<IAuthenticationService>();
            authenticationService.GetAccessToken().Returns("token");

            var client = new EcrmODataApiClientBuilder().Build(httpClient, authenticationService);

            var response = await client.GetWhoAmI();

            response.Should().NotBeNull();
        }
    }
}
