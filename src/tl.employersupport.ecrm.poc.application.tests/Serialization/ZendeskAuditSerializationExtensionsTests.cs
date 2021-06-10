using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.tests.Builders;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.tests.Serialization
{
    public class ZendeskAuditSerializationExtensionsTests
    {
        [Fact]
        public void Deserializes_Zendesk_Audits_Should_Return_Audits()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();

            audit.Id.Should().Be(790364969779);
            audit.TicketId.Should().Be(4485);
            audit.AuthorId.Should().Be(369756029380);
            audit.CreatedAt.Should()
                .Be(DateTime.Parse("2021-06-09T09:12:50Z",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_Events_Should_Have_Expected_Count()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();

            audit.Events.Should().NotBeNull();
            audit.Events.Length.Should().Be(16);
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_First_Event_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();
            var auditEvent = audit.Events.First();

            auditEvent.Id.Should().Be(790364969799);
            auditEvent.Type.Should().Be("Comment");
            auditEvent.AuthorId.Should().Be(369756029380);
            auditEvent.AuditId.Should().Be(790364969779);
            auditEvent.Body.Should().Be("I need information about industry placements.");
            auditEvent.HtmlBody.Should().Be("\u003Cdiv class=\u0022zd-comment\u0022 dir=\u0022auto\u0022\u003E\u003Cp dir=\u0022auto\u0022\u003EI need information about industry placements.\u003C/p\u003E\u003C/div\u003E");
            auditEvent.PlainBody.Should().Be("I need information about industry placements.");
            auditEvent.Public.Should().BeTrue();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_First_Event_Attachements_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();
            var auditEvent = audit.Events.First();

            auditEvent.Attachments.Should().NotBeNull();
            auditEvent.Attachments.Length.Should().Be(1);

            var attachment = auditEvent.Attachments.First();

            attachment.Id.Should().Be(371525559639);
            attachment.Url.Should().Be("https://tlevelsemployertest.zendesk.com/api/v2/attachments/371525559639.json");
            attachment.FileName.Should().Be("test_attachment.txt");
            attachment.ContentUrl.Should().Be("https://esfa1567428279.zendesk.com/attachments/token/eRUVY7atdA1NbyiRlLGFaQo77/?name=test_attachment.txt");
            attachment.MappedContentUrl.Should().Be("https://esfa1567428279.zendesk.com/attachments/token/eRUVY7atdA1NbyiRlLGFaQo77/?name=test_attachment.txt");
            attachment.ContentType.Should().Be("text/plain");
            attachment.Size.Should().Be(22);
            attachment.Width.Should().BeNull();
            attachment.Height.Should().BeNull();
            attachment.Inline.Should().BeFalse();
            attachment.Deleted.Should().BeFalse();
            attachment.Thumbnails.Should().NotBeNull();
            attachment.Thumbnails.Length.Should().Be(0);
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_Event_With_String_Type_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();
            var auditEvent = audit.Events.FirstOrDefault(ev => ev.Id == 790364970019);
            
            auditEvent.Should().NotBeNull();

            auditEvent!.Id.Should().Be(790364970019);
            auditEvent!.Type.Should().Be("Create");
            //TODO: Remove ToString?
            auditEvent!.Value.ToString().Should().Be("i_m_ready_to_offer_an_industry_placement");
            auditEvent!.FieldName.Should().Be("360020522600");
            auditEvent!.Body.Should().BeNull();
            auditEvent!.HtmlBody.Should().BeNull();
            auditEvent!.PlainBody.Should().BeNull();
            auditEvent!.Public.Should().BeFalse();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_Event_With_Complex_Type_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();
            var auditEvent = audit.Events.FirstOrDefault(ev => ev.Id == 790364969939);
            /*
          "value": [
            "1-9__micro_",
            "i_m_ready_to_offer_an_industry_placement",
            "tlevels-email",
            "tlevels_approved"
          ],
          "field_name": "tags"
             */
            auditEvent.Should().NotBeNull();

            auditEvent!.Id.Should().Be(790364969939);
            auditEvent!.Type.Should().Be("Create");
            auditEvent!.Value.Should().NotBeNull();
            //TODO: Validate value
            auditEvent!.FieldName.Should().Be("tags");
            auditEvent!.Body.Should().BeNull();
            auditEvent!.HtmlBody.Should().BeNull();
            auditEvent!.PlainBody.Should().BeNull();
            auditEvent!.Public.Should().BeFalse();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Audits_Via_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketAuditsResponse();

            var result = json.DeserializeZendeskAudits();

            var audit = result.First();

            audit.Via.Should().NotBeNull();
            audit.Via.Channel.Should().Be("web");
            audit.Via.Source.Should().NotBeNull();
            audit.Via.Source.From.Should().NotBeNull();
            audit.Via.Source.To.Should().NotBeNull();
        }
    }
}
