using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Model;
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

        private const string ZendeskApiBaseUri = "https://zendesk.test.api/v2/";
        private const string ActionUriFragment = "getticket";

        [Fact]
        public async Task TicketService_GetTicket_Returns_Expected_Value()
        {
            const int ticketId = 1;
            
            var ticketWithSideloadsUriFragment = $"tickets/{ticketId}.json?include={Sideloads.GetTicketSideloads()}";
            var ticketCommentsUriFragment = $"tickets/{ticketId}/comments.json";
            var ticketAuditsUriFragment = $"tickets/{ticketId}/audits.json";

            var ticketJson = JsonBuilder.BuildValidTicketWithSideloadsResponse();
            var ticketCommentsJson = JsonBuilder.BuildValidTicketWithSideloadsResponse();
            var ticketAuditJson = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var httpClient =
                new TestHttpClientFactory()
                    .CreateHttpClient(
                        new Uri(ZendeskApiBaseUri),
                        new Dictionary<Uri, string>
                        {
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketWithSideloadsUriFragment), ticketJson },
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketCommentsUriFragment), ticketCommentsJson },
                            { new Uri(new Uri(ZendeskApiBaseUri), ticketAuditsUriFragment), ticketAuditJson },
                        });
            httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(nameof(TicketService))
                .Returns(httpClient);

            var service = new TicketServiceBuilder().Build(httpClientFactory);
            
            var ticket = await service.GetTicket(ticketId);

            ticket.Id.Should().Be(ticketId);

        }
    }
    }
