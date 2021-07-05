using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.Serialization
{
    public class ZendeskCommentSerializationExtensionsTests
    {
        [Fact]
        public void Deserializes_Zendesk_Comments_Should_Return_Comments()
        {
            var json = JsonBuilder.BuildZendeskTicketCommentsResponse();

            var result = json.DeserializeZendeskComments();

            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Comments_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildZendeskTicketCommentsResponse();

            var result = json.DeserializeZendeskComments();

            var comment = result.First();

            comment.Id.Should().Be(790364969799);
            comment.Type.Should().Be("Comment");
            comment.AuthorId.Should().Be(369756029380);
            comment.Body.Should().Be("I need information about industry placements.");
            comment.HtmlBody.Should().Be("<div class=\"zd-comment\" dir=\"auto\"><p dir=\"auto\">I need information about industry placements.</p></div>");
            comment.PlainBody.Should().Be("I need information about industry placements.");
            comment.Public.Should().BeTrue();
            comment.AuditId.Should().Be(790364969779);
            comment.CreatedAt.Should()
                .Be(DateTime.Parse("2021-06-09T09:12:50Z",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_Comments_Attachments_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildZendeskTicketCommentsResponse();

            var result = json.DeserializeZendeskComments();

            var comment = result.First();

            comment.Attachments.Should().NotBeNull();
            comment.Attachments.Length.Should().Be(1);

            var attachment = comment.Attachments.First();

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
        public void SerializationExtensions_Deserialize_Zendesk_Comments_Via_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildZendeskTicketCommentsResponse();

            var result = json.DeserializeZendeskComments();

            var comment = result.First();

            comment.Via.Should().NotBeNull();
            comment.Via.Channel.Should().Be("web");
            comment.Via.Source.Should().NotBeNull();
            comment.Via.Source.From.Should().NotBeNull();
            comment.Via.Source.To.Should().NotBeNull();
        }
    }
}
