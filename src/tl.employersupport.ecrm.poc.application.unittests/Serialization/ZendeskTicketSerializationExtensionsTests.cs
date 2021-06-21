using System;
using System.Globalization;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.Serialization
{
    public class ZendeskTicketSerializationExtensionsTests
    {
        [Fact]
        public void Deserializes_Zendesk_Ticket_Should_Return_Ticket()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticket = json.DeserializeZendeskTicket();

            ticket.Should().NotBeNull();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Ticket_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticket = json.DeserializeZendeskTicket();

            ticket.Id.Should().Be(4485);
            ticket.Url.Should().Be("https://tlevelsemployertest.zendesk.com/api/v2/tickets/4485.json");
            ticket.Type.Should().BeNull();
            ticket.ExternalId.Should().BeNull();
            ticket.Description.Should().Be("I need information about industry placements.");
            ticket.Status.Should().Be("new");

            ticket.Recipient.Should().BeNull();
            ticket.RequesterId.Should().Be(369756029380);
            ticket.SubmitterId.Should().Be(369756029380);
            ticket.AssigneeId.Should().BeNull();
            ticket.OrganizationId.Should().Be(373080599360);
            ticket.GroupId.Should().BeNull();
            ticket.CollaboratorIds.Should().BeEmpty();
            ticket.FollowerIds.Should().BeEmpty();
            ticket.EmailCcIds.Should().BeEmpty();
            ticket.ForumTopicId.Should().BeNull();
            ticket.HasIncidents.Should().BeFalse();
            ticket.DueAt.Should().BeNull();
            ticket.Subject.Should().BeEmpty();
            ticket.RawSubject.Should().BeEmpty();
            ticket.Priority.Should().BeNull();
            ticket.ProblemId.Should().BeNull();
            ticket.IsPublic.Should().BeTrue();
            ticket.SatisfactionProbability.Should().BeNull();

            ticket.FollowupIds.Should().BeEmpty();
            ticket.TicketFormId.Should().Be(360001820480);
            ticket.BrandId.Should().Be(360002505739);
            ticket.SatisfactionProbability.Should().BeNull();
            ticket.AllowChannelBack.Should().BeFalse();
            ticket.AllowAttachments.Should().BeTrue();

            //TODO: ticket.Fields
            //TODO: ticket.CustomFields
            ticket.CommentCount.Should().Be(1);

            ticket.SatisfactionRating.Should().NotBeNull();
            ticket.SatisfactionRating.Score.Should().Be("unoffered");

            ticket.CreatedAt.Should()
                .Be(DateTime.Parse("2021-06-09T09:12:50Z",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));

            ticket.UpdatedAt.Should()
                .Be(DateTime.Parse("2021-06-09T09:12:50Z",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Ticket_Tags_Should_Have_Expected_Values()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticket = json.DeserializeZendeskTicket();

            ticket.Tags.Should().NotBeNull();
            ticket.Tags.Length.Should().Be(4);

            ticket.Tags.Should().Contain("1-9__micro_");
            ticket.Tags.Should().Contain("i_m_ready_to_offer_an_industry_placement");
            ticket.Tags.Should().Contain("tlevels-email");
            ticket.Tags.Should().Contain("tlevels_approved");
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Ticket_Fields_Should_Have_Expected_Values()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticket = json.DeserializeZendeskTicket();

            ticket.Fields.Should().NotBeNull();
            ticket.Fields.Length.Should().Be(99);

            //Spot check
            ticket.Fields.Should().Contain(f =>
                f.Id == 360020522620 &&
                f.Value == "Mike\u0027s Emporium");
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Ticket_CustomFields_Should_Have_Expected_Values()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticket = json.DeserializeZendeskTicket();

            ticket.CustomFields.Should().NotBeNull();
            ticket.CustomFields.Length.Should().Be(99);

            //Spot check
            ticket.CustomFields.Should().Contain(f =>
                f.Id == 360019751459 &&
                f.Value == "Mike");
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Ticket_Via_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticket = json.DeserializeZendeskTicket();

            ticket.Via.Should().NotBeNull();
            ticket.Via.Channel.Should().Be("web");
            ticket.Via.Source.Should().NotBeNull();
            ticket.Via.Source.From.Should().NotBeNull();
            ticket.Via.Source.To.Should().NotBeNull();
        }
    }
}
