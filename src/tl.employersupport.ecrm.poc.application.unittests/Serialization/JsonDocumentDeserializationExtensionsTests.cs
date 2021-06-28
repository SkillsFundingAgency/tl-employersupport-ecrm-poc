using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.Serialization
{
    public class JsonDocumentDeserializationExtensionsTests
    {
        [Fact]
        public void Deserialize_DeserializeTicketFields_Returns_Expected_Value()
        {
            var jsonDocument = JsonDocument.Parse(
                JsonBuilder.BuildTicketFieldsResponse());

            var fields = jsonDocument.DeserializeTicketFields();

            fields.Should().NotBeNullOrEmpty();
            fields.Count.Should().Be(168);

            const long testKey = 360019751459;
            fields.Should().ContainKey(testKey);
            fields[testKey].Id.Should().Be(testKey);
            fields[testKey].Title.Should().Be("T Level Name");
            fields[testKey].Type.Should().Be("text");
            fields[testKey].Active.Should().BeTrue();
        }

        [Fact]
        public void Deserialize_ToEmployerContactTicket_Returns_Expected_Value()
        {
            var fieldDefinitions = JsonDocument.Parse(
                    JsonBuilder.BuildTicketFieldsResponse())
                .DeserializeTicketFields();

            var jsonDocument = JsonDocument.Parse(
                JsonBuilder.BuildTicketWithSideloadsResponse());

            var ticket = jsonDocument.ToEmployerContactTicket(fieldDefinitions);
            ticket.Should().NotBeNull();

            const int ticketId = 4485;
            ticket.Id.Should().Be(ticketId);
            ticket.Description.Should().Be("I need information about industry placements.");

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
            ticket.ContactName.Should().Be("Mike");
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
        public void Deserialize_ToOrganisationDetailList_Returns_Expected_Value()
        {
            var jsonDocument = JsonDocument.Parse(
                JsonBuilder.BuildTicketWithSideloadsResponse());

            var organisations = jsonDocument.DeserializeOrganisationDetails();
            organisations.Should().NotBeNullOrEmpty();
            organisations.Count.Should().Be(1);

            var organisation = organisations[0];
            organisation.Id.Should().Be(373080599360);
            organisation.Name.Should().Be("Digital Education");
        }

        [Fact]
        public void Deserialize_ToTicketSearchResultList_Returns_Expected_Value()
        {
            var jsonDocument = JsonDocument.Parse(
                JsonBuilder.BuildTicketSearchResultsResponse());

            var searchResults = jsonDocument.ToTicketSearchResultList();
            searchResults.Should().NotBeNull();
            searchResults.Should().NotBeEmpty();

            searchResults.Count.Should().Be(31);

            const long testId = 4528;
            var searchResult = searchResults.FirstOrDefault(s => s.Id == testId);
            searchResult.Should().NotBeNull();
            searchResult!.Id.Should().Be(testId);
            searchResult.Subject.Should().Be("T Levels and industry placement support for employers - Online query");
        }

        [Fact]
        public void Deserialize_ToUserDetailList_Returns_Expected_Value()
        {
            var jsonDocument = JsonDocument.Parse(
                JsonBuilder.BuildTicketWithSideloadsResponse());

            var users = jsonDocument.DeserializeUserDetails();
            users.Should().NotBeNull();
            users.Count.Should().Be(1);

            var user = users[0];
            user.Id.Should().Be(369756029380);
            user.Name.Should().Be("Mike Wild");
            user.Email.Should().Be("mike.wild@digital.education.gov.uk");
        }

        [Fact]
        public void Deserialize_ExtractTicketSafeTags_Returns_Expected_Value()
        {
            var jsonDocument = JsonDocument.Parse(
                JsonBuilder.BuildTicketResponse());

            var safeTags = jsonDocument.ExtractTicketSafeTags();

            safeTags.Should().NotBeNull();
            safeTags.Tags.Count.Should().Be(4);

            safeTags.Tags.Should().Contain("1-9__micro_");
            safeTags.Tags.Should().Contain("i_m_ready_to_offer_an_industry_placement");
            safeTags.Tags.Should().Contain("tlevels-email");
            safeTags.Tags.Should().Contain("tlevels_approved");

            safeTags.SafeUpdate.Should().BeTrue();
            safeTags.UpdatedStamp.Should().Be(DateTimeOffset.Parse("2021-06-09T09:12:50Z",
                styles: DateTimeStyles.AdjustToUniversal));
        }
    }
}
