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

            var client = new EcrmODataApiClientBuilder().Build(
                httpClient,
                new AuthenticationServiceBuilder().Build());

            var response = await client.GetWhoAmI();

            response.Should().NotBeNull();
        }

        [Fact]
        public async Task EcrmODataApiClient_GetAccount_Returns_Expected_Value()
        {
            var accountJson = JsonBuilder.BuildEcrmAccount();
            var accountId = Guid.Parse("7e82dc4d-e846-4560-bb76-b5255c18fc59");

            //TODO: Move to query builder class
            // ReSharper disable StringLiteralTypo
            var query =
                $"$select=accountid,name,accountnumber,address1_primarycontactname,address1_line1,address1_line2,address1_line3,address1_postalcode,address1_city,telephone1,customersizecode&$orderby=name desc&$filter=accountid eq '{accountId:D}'";
            // ReSharper restore StringLiteralTypo

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _ecrmODataApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                //new Uri(_ecrmODataApiBaseUri, $"accounts?{query}"), accountJson
                                new Uri(_ecrmODataApiBaseUri, $"accounts({accountId:D})"), accountJson
                            }
                        });

            var client = new EcrmODataApiClientBuilder().Build(
                httpClient, 
                new AuthenticationServiceBuilder().Build());

            var response = await client.GetAccount(accountId);

            response.Should().NotBeNull();
        }
    }
}
