using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using tl.employersupport.ecrm.poc.tests.common.HttpClient;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.ApiClients
{
    public class ZendeskApiClientTests
    {
        private const string ZendeskApiBaseUrl = "https://zendesk.test.api/v2/";
        private readonly Uri _zendeskApiBaseUri = new(ZendeskApiBaseUrl);

        [Fact]
        public void ZendeskApiClient_Constructor_Succeeds_With_Valid_Parameters()
        {
            var _ = new ZendeskApiClientBuilder().Build();
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void ZendeskApiClient_Constructor_Guards_Against_NullParameters()
        {
            typeof(ZendeskApiClient)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void ZendeskApiClient_Constructor_Guards_Against_BadParameters()
        {
            typeof(ZendeskApiClient)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task ZendeskApiClient_GetTicketJsonDocument_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var ticketJson = JsonBuilder.BuildTicketWithSideloadsResponse();
            var ticketCommentsJson = JsonBuilder.BuildTicketCommentsResponse();
            var ticketAuditsJson = JsonBuilder.BuildTicketAuditsResponse();
            var ticketFieldJson = JsonBuilder.BuildTicketFieldsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_zendeskApiBaseUri,
                                    $"tickets/{ticketId}.json?include={Sideloads.GetTicketSideloads()}"),
                                ticketJson
                            },
                            {
                                new Uri(_zendeskApiBaseUri,
                                    $"tickets/{ticketId}/comments.json"),
                                ticketCommentsJson
                            },
                            {
                                new Uri(_zendeskApiBaseUri,
                                    $"tickets/{ticketId}/audits.json"),
                                ticketAuditsJson
                            },
                            {
                                new Uri(_zendeskApiBaseUri,
                                    "ticket_fields.json"),
                                ticketFieldJson
                            }
                        });

            var client = new ZendeskApiClientBuilder().Build(httpClient);
            var sideloads = Sideloads.GetTicketSideloads();

            var jsonDocument = await client.GetTicketJsonDocument(ticketId, sideloads);

            jsonDocument.Should().NotBeNull();
        }

        [Fact]
        public async Task ZendeskApiClient_GetTicketJson_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var ticketJson = JsonBuilder.BuildTicketWithSideloadsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_zendeskApiBaseUri,
                                    $"tickets/{ticketId}.json?include={Sideloads.GetTicketSideloads()}"),
                                ticketJson
                            }
                        });
            
            var client = new ZendeskApiClientBuilder().Build(httpClient);
            var sideloads = Sideloads.GetTicketSideloads();

            var jsonDocument = await client.GetTicketJson(ticketId, sideloads);

            jsonDocument.Should().NotBeNull();
        }

        [Fact]
        public async Task ZendeskApiClient_GetTicketCommentsJson_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var ticketCommentsJson = JsonBuilder.BuildTicketCommentsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_zendeskApiBaseUri,
                                    $"tickets/{ticketId}/comments.json"),
                                ticketCommentsJson
                            }
                        });

            var client = new ZendeskApiClientBuilder().Build(httpClient);

            var jsonDocument = await client.GetTicketCommentsJson(ticketId);

            jsonDocument.Should().NotBeNull();
        }

        [Fact]
        public async Task ZendeskApiClient_GetTicketAuditsJson_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var ticketAuditsJson = JsonBuilder.BuildTicketAuditsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {
                                new Uri(_zendeskApiBaseUri,
                                    $"tickets/{ticketId}/audits.json"),
                                ticketAuditsJson
                            }
                        });

            var client = new ZendeskApiClientBuilder().Build(httpClient);

            var json = await client.GetTicketAuditsJson(ticketId);

            json.Should().NotBeNull();
        }

        [Fact]
        public async Task ZendeskApiClient_GetTicketFieldsJsonDocument_Returns_Expected_Value()
        {
            var ticketFieldsJson = JsonBuilder.BuildTicketFieldsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            {new Uri(_zendeskApiBaseUri, "ticket_fields.json"), ticketFieldsJson}
                        });

            var client = new ZendeskApiClientBuilder().Build(httpClient);

            var jsonDocument = await client.GetTicketFieldsJsonDocument();

            jsonDocument.Should().NotBeNull();
        }

        [Fact]
        public async Task ZendeskApiClient_GetTicketSearchResultsJsonDocument_Returns_Expected_Value()
        {
            const string query = "type:ticket status:new brand:tlevelsemployertest";
            var ticketSearchUriFragment = $"search.json?query={WebUtility.UrlEncode(query)}";

            var ticketSearchResultsJson = JsonBuilder.BuildTicketSearchResultsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            { new Uri(_zendeskApiBaseUri, ticketSearchUriFragment), ticketSearchResultsJson }
                        });

            var client = new ZendeskApiClientBuilder().Build(httpClient);

            var jsonDocument = await client.GetTicketSearchResultsJsonDocument(query);

            jsonDocument.Should().NotBeNull();
        }

        [Fact]
        public async Task ZendeskApiClient_PostTags_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var postTagsUriFragment = $"tickets/{ticketId}/tags.json";
            var tagsJson = JsonBuilder.BuildTagsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            { new Uri(_zendeskApiBaseUri, postTagsUriFragment), tagsJson }
                        });

            var tags = new SafeTags
            {
                Tags = new[] { "tag1", "tag2" },
                UpdatedStamp = new DateTimeOffset(2019, 09, 12, 21, 45, 16, TimeSpan.Zero)
            };

            var client = new ZendeskApiClientBuilder().Build(httpClient);

            var jsonDocument = await client.PostTags(ticketId, tags);

            jsonDocument.Should().NotBeNull();
        }
        
        [Fact]
        public async Task ZendeskApiClient_PutTags_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var postTagsUriFragment = $"tickets/{ticketId}/tags.json";
            var tagsJson = JsonBuilder.BuildTagsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        _zendeskApiBaseUri,
                        new Dictionary<Uri, string>
                        {
                            { new Uri(_zendeskApiBaseUri, postTagsUriFragment), tagsJson }
                        });

            var tags = new SafeTags
            {
                Tags = new[] { "tag1", "tag2" },
                UpdatedStamp = new DateTimeOffset(2019, 09, 12, 21, 45, 16, TimeSpan.Zero)
            };

            var client = new ZendeskApiClientBuilder().Build(httpClient);

            var jsonDocument = await client.PutTags(ticketId, tags);

            jsonDocument.Should().NotBeNull();
        }
    }
}
