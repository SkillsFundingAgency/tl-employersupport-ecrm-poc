using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.tests.Builders;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.tests.Serialization
{
    public class ZendeskTicketResponseSerializationExtensionsTests
    {
        [Fact]
        public void Deserializes_Zendesk_TicketResponse_Should_Return_TicketResponse()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Should().NotBeNull();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_TicketResponse_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildValidTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Ticket.Should().NotBeNull();
            ticketResponse.Groups.Should().NotBeNull();
            ticketResponse.Users.Should().NotBeNull();
            ticketResponse.Organizations.Should().NotBeNull();

            //ticketResponse.Groups.Should().NotBeEmpty();
            ticketResponse.Users.Should().NotBeEmpty();
            ticketResponse.Organizations.Should().NotBeEmpty();
        }
    }
}
