using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.application.tests.Builders;
using tl.employersupport.ecrm.poc.application.tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace tl.employersupport.ecrm.poc.application.tests
{
    public class EmailServiceTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper _output;

        public EmailServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EmailService_Constructor_Succeeds_With_Valid_Parameters()
        {
            var config = Substitute.For<IOptions<EmailConfiguration>>();
            config.Value.Returns(new EmailConfiguration());

            var _ = new EmailServiceBuilder().Build(config);
            //Test passes if no exceptions thrown
        }

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
