using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.application.unittests.Builders;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.Services
{
    public class EmailServiceTests
    {
        [Fact]
        public void EmailService_Constructor_Guards_Against_NullParameters()
        {
            typeof(EmailService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void EmailService_Constructor_Guards_Against_BadParameters()
        {
            typeof(EmailService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task EmailService_SendZendeskTicketCreatedEmail_Returns_Expected_Value()
        {
            const int ticketId = 4485;
            var createdAt = new DateTime(2021, 06, 15, 11, 11, 11);

            var config = Substitute.For<IOptions<EmailConfiguration>>();
            config.Value.Returns(new EmailConfiguration
            {
                GovNotifyApiKey = "test",
                SupportEmailAddress = "tester@test.com",
                ZendeskTicketCreatedEmailTemplateId = "0A969655-25D7-4EE0-9208-E72F56CF587A"
            });

            var service = new EmailServiceBuilder().Build(config);

            var success = await service.SendZendeskTicketCreatedEmail(ticketId, createdAt);

            success.Should().BeTrue();
        }
    }
}
