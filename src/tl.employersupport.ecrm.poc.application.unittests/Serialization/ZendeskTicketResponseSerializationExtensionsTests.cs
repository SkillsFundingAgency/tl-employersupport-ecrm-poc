using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.Serialization
{
    public class ZendeskTicketResponseSerializationExtensionsTests
    {
        [Fact]
        public void Deserializes_Zendesk_TicketResponse_Should_Return_TicketResponse()
        {
            var json = JsonBuilder.BuildTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Should().NotBeNull();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_TicketResponse_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Ticket.Should().NotBeNull();
            ticketResponse.Groups.Should().NotBeNull();
            ticketResponse.Users.Should().NotBeNull();
            ticketResponse.Organizations.Should().NotBeNull();

            //ticketResponse.Groups.Should().NotBeEmpty();
            ticketResponse.Users.Should().NotBeEmpty();
            ticketResponse.Organizations.Should().NotBeEmpty();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_TicketResponse_Organizations_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Organizations.Should().NotBeNull();

            ticketResponse.Organizations.Length.Should().Be(1);
            var organization = ticketResponse.Organizations[0];

            organization.Id.Should().Be(373080599360);
            organization.Name.Should().Be("Digital Education");
            organization.Url.Should()
                .Be("https://tlevelsemployertest.zendesk.com/api/v2/organizations/373080599360.json");

            organization.OrganizationFields.Should().NotBeNull();
            organization.OrganizationFields.AddressLine1.Should().BeNull();
            organization.OrganizationFields.AddressLine2.Should().BeNull();
            organization.OrganizationFields.AddressLine3.Should().BeNull();
            organization.OrganizationFields.City.Should().BeNull();
            organization.OrganizationFields.Postcode.Should().BeNull();
            organization.OrganizationFields.OrganisationType.Should().BeNull();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_TicketResponse_User_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Users.Should().NotBeNull();

            ticketResponse.Users.Length.Should().Be(1);
            var user = ticketResponse.Users[0];

            user.Id.Should().Be(369756029380);
            user.Name.Should().Be("Mike Wild");
            user.Email.Should().Be("mike.wild@digital.education.gov.uk");
            user.Url.Should().Be("https://tlevelsemployertest.zendesk.com/api/v2/users/369756029380.json");

            user.UserFields.Should().NotBeNull();
            user.UserFields.AddressLine1.Should().BeNull();
            user.UserFields.AddressLine2.Should().BeNull();
            user.UserFields.AddressLine3.Should().BeNull();
            user.UserFields.City.Should().BeNull();
            user.UserFields.Postcode.Should().BeNull();
        }

        [Fact]
        public void SerializationExtensions_Deserialize_Zendesk_TicketResponse_Groups_Should_Have_Expected_Members()
        {
            var json = JsonBuilder.BuildTicketWithSideloadsResponse();

            var ticketResponse = json.DeserializeZendeskTicketResponse();

            ticketResponse.Groups.Should().NotBeNull();
            ticketResponse.Groups.Length.Should().Be(0);

            //TODO: Add test data with group
        }
    }
}
