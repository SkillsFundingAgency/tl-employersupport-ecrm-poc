using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using tl.employersupport.ecrm.poc.tests.common.HttpClient;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.ApiClients
{
    public class EcrmODataApiClientTests
    {
        private const string EcrmApiBaseUrl = "https://test.api.crm4.dynamics.com/api/data/v9.2/";
        private readonly Uri _ecrmApiBaseUri = new(EcrmApiBaseUrl);

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
                        _ecrmApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_ecrmApiBaseUri, "WhoAmI"), whoAmIJson
                            }
                        });

            var client = new EcrmODataApiClientBuilder().Build(
                httpClient);

            var whoAmIExpected = JsonSerializer
                .Deserialize<WhoAmI>(whoAmIJson);

            var whoAmI = await client.GetWhoAmI();

            whoAmI.Should().NotBeNull();
            whoAmI.BusinessUnitId.Should().Be(whoAmIExpected!.BusinessUnitId);
            whoAmI.OrganizationId.Should().Be(whoAmIExpected.OrganizationId);
            whoAmI.UserId.Should().Be(whoAmIExpected.UserId);
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
                        _ecrmApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                //new Uri(_ecrmODataApiBaseUri, $"accounts?{query}"), accountJson
                                new Uri(_ecrmApiBaseUri, $"accounts({accountId:D})"), accountJson
                            }
                        });

            var client = new EcrmODataApiClientBuilder().Build(
                httpClient);

            var response = await client.GetAccount(accountId);

            response.Should().NotBeNull();
        }
    }
}
