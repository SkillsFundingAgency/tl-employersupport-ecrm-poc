using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using tl.employersupport.ecrm.poc.tests.common.HttpClient;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.ApiClients
{
    public class EcrmApiClientTests
    {
        private const string EcrmApiBaseUrl = "https://test.ecrm.api/v2/";
        private readonly Uri _ecrmApiBaseUri = new(EcrmApiBaseUrl);

        [Fact]
        public void EcrmApiClient_Constructor_Succeeds_With_Valid_Parameters()
        {
            var _ = new EcrmApiClientBuilder().Build();
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void EcrmApiClient_Constructor_Guards_Against_NullParameters()
        {
            typeof(EcrmApiClient)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void EcrmApiClient_Constructor_Guards_Against_BadParameters()
        {
            typeof(EcrmApiClient)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task EcrmApiClient_GetHeartbeat_Returns_Expected_Value()
        {
            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _ecrmApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_ecrmApiBaseUri, "HeartBeat"), ""
                            }
                        });

            var client = new EcrmApiClientBuilder().Build(httpClient);

            var response = await client.GetHeartbeat();

            response.Should().BeTrue();
        }

        [Fact]
        public async Task EcrmApiClient_GetEmployer_Returns_Expected_Value()
        {
            var searchRequest = new EmployerSearchRequestBuilder()
                .Build();

            var response = new EmployerBuilder()
                .Build();

            var responseJson = JsonSerializer.Serialize(response,
                JsonExtensions.DefaultJsonSerializerOptions);

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _ecrmApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_ecrmApiBaseUri, "employerSearch"),
                                responseJson
                            }
                        });

            var client = new EcrmApiClientBuilder().Build(httpClient);

            var employer = await client.GetEmployer(searchRequest);

            employer.Should().NotBeNull();
            employer.AccountId.Should().NotBe(Guid.Empty);
            employer.CompanyName.Should().Be(searchRequest.CompanyName);
        }
    }
}
