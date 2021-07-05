using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace tl.employersupport.ecrm.poc.application.unittests.Services
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

            var _ = new TicketServiceBuilder().Build(configuration: config);
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
                () => new TicketServiceBuilder().Build(
                    Substitute.For<IZendeskApiClient>(),
                    Substitute.For<ILogger<TicketService>>(),
                    Substitute.For<IOptions<ZendeskConfiguration>>()));
        }

        [Fact]
        public async Task TicketService_GetEmployerContactTicket_Returns_Expected_Value()
        {
            const int ticketId = 4485;

            var ticketJson = JsonBuilder.BuildZendeskTicketWithSideloadsResponse();
            var ticketFieldsJson = JsonBuilder.BuildZendeskTicketFieldsResponse();

            var ticketJsonDocument = JsonDocument.Parse(ticketJson);
            var ticketFieldsJsonDocument = JsonDocument.Parse(ticketFieldsJson);

            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.GetTicketJsonDocument(ticketId, Arg.Is<string>(s => s != null))
                .Returns(ticketJsonDocument);
            apiClient.GetTicketFieldsJsonDocument()
                .Returns(ticketFieldsJsonDocument);

            var service = new TicketServiceBuilder().Build(apiClient);

            var ticket = await service.GetEmployerContactTicket(ticketId);

            ticket.Should().NotBeNull();
            ticket.Id.Should().Be(ticketId);

            ticket.Message.Should().Be("I need information about industry placements.");

            ticket.CreatedAt.Should().Be(
                DateTimeOffset.Parse("2021-06-09T09:12:50Z",
                    styles: DateTimeStyles.AdjustToUniversal));

            ticket.UpdatedAt.Should().Be(
                DateTimeOffset.Parse("2021-06-09T09:12:50Z",
                    styles: DateTimeStyles.AdjustToUniversal));

            ticket.RequestedBy.Should().NotBeNull();
            ticket.RequestedBy.Id.Should().Be(369756029380);
            ticket.RequestedBy.Name.Should().Be("Mike Wild");
            ticket.RequestedBy.Email.Should().Be("mike.wild@digital.education.gov.uk");

            ticket.Organisation.Should().NotBeNull();
            ticket.Organisation.Id.Should().Be(373080599360);
            ticket.Organisation.Name.Should().Be("Digital Education");

            ticket.EmployerName.Should().Be("Mike's Emporium");
            ticket.Postcode.Should().BeNullOrEmpty();
            ticket.AddressLine1.Should().BeNullOrEmpty();
            ticket.ContactName.Should().Be("Mike Wild");
            ticket.ContactFirstName.Should().Be("Mike");
            ticket.ContactLastName.Should().Be("Wild");
            ticket.Phone.Should().Be("077123456789");
            ticket.EmployerSize.Should().Be("1-9__micro_");
            ticket.ContactMethod.Should().Be("tlevels-email");
            ticket.ContactReason.Should().Be("i_m_ready_to_offer_an_industry_placement");
            ticket.QuerySubject.Should().BeNullOrEmpty();

            ticket.Tags.Count.Should().Be(4);

            ticket.Tags.Should().NotBeNullOrEmpty();
            ticket.Tags.Count.Should().Be(4);
            ticket.Tags.Should().Contain("1-9__micro_");
            ticket.Tags.Should().Contain("i_m_ready_to_offer_an_industry_placement");
            ticket.Tags.Should().Contain("tlevels-email");
            ticket.Tags.Should().Contain("tlevels_approved");
        }

        [Fact]
        public async Task TicketService_GetTicket_Returns_Expected_Value()
        {
            const int ticketId = 4485;
            var sideloads = Sideloads.GetTicketSideloads();
            var ticketJson = JsonBuilder.BuildZendeskTicketWithSideloadsResponse();
            var ticketCommentsJson = JsonBuilder.BuildZendeskTicketCommentsResponse();
            var ticketAuditsJson = JsonBuilder.BuildZendeskTicketAuditsResponse();

            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.GetTicketJson(ticketId, sideloads)
                .Returns(ticketJson);
            apiClient.GetTicketCommentsJson(ticketId)
                .Returns(ticketCommentsJson);
            apiClient.GetTicketAuditsJson(ticketId)
                .Returns(ticketAuditsJson);

            var service = new TicketServiceBuilder().Build(apiClient);

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
            var ticketFieldsJson = JsonBuilder.BuildZendeskTicketFieldsResponse();
            var ticketJsonDocument = JsonDocument.Parse(ticketFieldsJson);

            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.GetTicketFieldsJsonDocument()
                .Returns(ticketJsonDocument);

            var service = new TicketServiceBuilder().Build(apiClient);

            var result = await service.GetTicketFields();

            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(168);

            const long testFieldId = 360020522620;
            const string testFieldName = "T Levels Employers - Company Name";
            result.Keys.Should().Contain(testFieldId);
            result[testFieldId].Title.Should().Be(testFieldName);
            result[testFieldId].Type.Should().Be("text");
            result[testFieldId].Active.Should().Be(true);
        }

        [Fact]
        public async Task TicketService_SearchTickets_Returns_Expected_Value()
        {
            const string query = "type:ticket status:new brand:tlevelsemployertest";

            var ticketSearchResultsJson = JsonBuilder.BuildZendeskTicketSearchResultsResponse();

            var ticketJsonDocument = JsonDocument.Parse(ticketSearchResultsJson);

            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.GetTicketSearchResultsJsonDocument(query)
                .Returns(ticketJsonDocument);

            var service = new TicketServiceBuilder().Build(apiClient);

            var results = await service.SearchTickets();

            results.Should().NotBeNullOrEmpty();
            results.Count.Should().BeGreaterOrEqualTo(1);
            results.Should().Contain(x => x.Id == 4485);
        }

        [Fact]
        public async Task TicketService_GetTicketTags_Returns_Expected_Value()
        {
            const int ticketId = 4485;
            var ticketJson = JsonBuilder.BuildZendeskTicketResponse();
            var ticketJsonDocument = JsonDocument.Parse(ticketJson);

            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.GetTicketJsonDocument(ticketId)
                .Returns(ticketJsonDocument);

            var service = new TicketServiceBuilder().Build(apiClient);

            var result = await service.GetTicketTags(ticketId);

            result.Should().NotBeNull();

            var updatedDate = DateTimeOffset.Parse("2021-06-09T09:12:50Z",
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

            var tagsJson = JsonBuilder.BuildZendeskTagsResponse();
            
            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.PutTags(ticketId, Arg.Is<SafeTags>(t => t != null))
                .Returns(JsonDocument.Parse(tagsJson));

            var service = new TicketServiceBuilder().Build(apiClient);

            var ticket = new CombinedTicket
            {
                Id = ticketId,
                Ticket = new Ticket
                {
                    Id = ticketId,
                    Tags = new[] { "tag1", "tag2" },
                    //UpdatedAt = DateTimeOffset.Parse("2019-09-12T21:45:16Z")
                    UpdatedAt = new DateTimeOffset(2019, 09, 12, 21, 45, 16, TimeSpan.Zero)
                },
            };

            await service.AddTag(ticketId, ticket, tag);

            //result.Should().NotBeNull();
        }

        [Fact]
        public async Task TicketService_ModifyTags_Works_As_Expected()
        {
            const int ticketId = 4485;

            var tagsJson = JsonBuilder.BuildZendeskTagsResponse();

            var apiClient = Substitute.For<IZendeskApiClient>();
            apiClient.PostTags(ticketId, Arg.Is<SafeTags>(t => t != null))
                .Returns(JsonDocument.Parse(tagsJson));

            var service = new TicketServiceBuilder().Build(apiClient);

            var tags = new SafeTags
            {
                Tags = new[] { "tag1", "tag2" },
                UpdatedStamp = new DateTimeOffset(2019, 09, 12, 21, 45, 16, TimeSpan.Zero)
            };

            await service.ModifyTags(ticketId, tags);
        }
    }
}
