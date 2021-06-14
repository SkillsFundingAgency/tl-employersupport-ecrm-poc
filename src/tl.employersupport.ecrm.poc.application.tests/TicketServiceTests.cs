using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.application.tests.Builders;
using tl.employersupport.ecrm.poc.application.tests.Extensions;
using tl.employersupport.ecrm.poc.application.tests.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace tl.employersupport.ecrm.poc.application.tests
{
    public class TicketServiceTests
    {
        private const string ZendeskApiBaseUri = "https://zendesk.test.api/v2/";

        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper _output;

        public TicketServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TicketService_Constructor_Succeeds_With_Valid_Parameters()
        {
            var config = Substitute.For<IOptions<ZendeskConfiguration>>();
            config.Value.Returns(new ZendeskConfiguration());

            var _ = new TicketServiceBuilder().Build(zendeskConfiguration: config);
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void TicketService_Constructor_Guards_Against_NullParameters()
        {
            typeof(TicketService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void TicketService_Constructor_Guards_Against_BadParameters()
        {
            typeof(TicketService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public void TicketService_Constructor_Guards_Against_Bad_Configuration()
        {
            Assert.Throws<ArgumentNullException>(
                "zendeskConfiguration.Value",
                () =>
                    new TicketService(
                        Substitute.For<IHttpClientFactory>(),
                        Substitute.For<ILogger<TicketService>>(),
                        Substitute.For<IOptions<ZendeskConfiguration>>())
            );
        }

        [Fact]
        public async Task TicketService_GetTicket_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var ticketWithSideloadsUriFragment = $"tickets/{ticketId}.json?include={Sideloads.GetTicketSideloads()}";
            var ticketCommentsUriFragment = $"tickets/{ticketId}/comments.json";
            var ticketAuditsUriFragment = $"tickets/{ticketId}/audits.json";

            var ticketJson = JsonBuilder.BuildValidTicketWithSideloadsResponse();
            var ticketCommentsJson = JsonBuilder.BuildValidTicketCommentsResponse();
            var ticketAuditJson = JsonBuilder.BuildValidTicketAuditsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        new Uri(ZendeskApiBaseUri),
                        new Dictionary<Uri, string>
                        {
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketWithSideloadsUriFragment), ticketJson },
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketCommentsUriFragment), ticketCommentsJson },
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketAuditsUriFragment), ticketAuditJson }
                        });
            httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(nameof(TicketService))
                .Returns(httpClient);

            var service = new TicketServiceBuilder().Build(httpClientFactory);

            var ticket = await service.GetTicket(ticketId);

            ticket.Id.Should().Be(ticketId);
            ticket.Ticket.Should().NotBeNull();
            ticket.Audits.Should().NotBeNull();
            ticket.Comments.Should().NotBeNull();
            ticket.Groups.Should().NotBeNull();
            ticket.Users.Should().NotBeNull();
            ticket.Organizations.Should().NotBeNull();
        }

        [Fact]
        public async Task TicketService_GetTicketFields_Returns_Expected_Value()
        {
            //var ticketFieldJson = JsonBuilder.BuildValidTicketFieldsResponse;

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        new Uri(ZendeskApiBaseUri),
                        new Dictionary<Uri, string>
                        {
                            //{ new Uri(new Uri(ZendeskApiBaseUri), "ticket_fields"), ticketFieldJson }
                        });
            httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(nameof(TicketService))
                .Returns(httpClient);

            var service = new TicketServiceBuilder().Build(httpClientFactory);

            var result = await service.GetTicketFields();

            result.Should().NotBeNullOrEmpty();
            result.Keys.Should().Contain(12345);
            result[12345].Should().Be("My Field");
            //TODO: get sample data json, load it and check results
        }

        [Fact]
        public async Task TicketService_SearchTickets_Returns_Expected_Value()
        {
            //var ticketSearchResultsJson = JsonBuilder.BuildValidTicketSearchResultsResponse;

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        new Uri(ZendeskApiBaseUri),
                        new Dictionary<Uri, string>
                        {
                            //{ new Uri(new Uri(ZendeskApiBaseUri), ticketUriFragment), ticketJson }
                        });
            httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(nameof(TicketService))
                .Returns(httpClient);

            var service = new TicketServiceBuilder().Build(httpClientFactory);

            var result = await service.SearchTickets();

            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(3);
        }

        [Fact]
        public async Task TicketService_GetTicketTags_Returns_Expected_Value()
        {
            const int ticketId = 4485;
            var ticketUriFragment = $"tickets/{ticketId}.json";
            var ticketJson = JsonBuilder.BuildValidTicketResponse();
            
            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        new Uri(ZendeskApiBaseUri),
                        new Dictionary<Uri, string>
                        {
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketUriFragment), ticketJson }
                        });
            httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(nameof(TicketService))
                .Returns(httpClient);

            var service = new TicketServiceBuilder().Build(httpClientFactory);

            var result = await service.GetTicketTags(ticketId);

            result.Should().NotBeNull();

            var updatedDate = DateTimeOffset.Parse( "2021-06-09T09:12:50Z", 
                styles: DateTimeStyles.AdjustToUniversal);
            result.UpdatedStamp.Should().Be(updatedDate);
            result.SafeUpdate.Should().BeTrue();

            result.Tags.Count.Should().Be(4);
            result.Tags.Should().Contain("1-9__micro_");
            result.Tags.Should().Contain("i_m_ready_to_offer_an_industry_placement");
            result.Tags.Should().Contain("tlevels-email");
            result.Tags.Should().Contain("tlevels_approved");
        }

        [Fact]
        public async Task TicketService_AddTag_Works_As_Expected()
        {
            const int ticketId = 4485;
            const string tag = "test_tag";

            var putTagsUriFragment = $"tickets/{ticketId}/tags.json";
            var tagsJson = JsonBuilder.BuildValidTagsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        new Uri(ZendeskApiBaseUri),
                        new Dictionary<Uri, string>
                        {
                            { new Uri(new Uri(ZendeskApiBaseUri), putTagsUriFragment), tagsJson }
                        });
            httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(nameof(TicketService))
                .Returns(httpClient);

            var service = new TicketServiceBuilder().Build(httpClientFactory);

            var ticket = new CombinedTicket
            {
                Id = ticketId,
                Ticket = new Ticket
                {
                    Id = ticketId,
                    Tags = new [] { "tag1", "tag2" },
                    //UpdatedAt = DateTimeOffset.Parse("2019-09-12T21:45:16Z")
                    UpdatedAt = new DateTimeOffset(2019,09,12,21,45,16,TimeSpan.Zero)
                },
            };

            await service.AddTag(ticketId, ticket, tag);
        }
    }
}
