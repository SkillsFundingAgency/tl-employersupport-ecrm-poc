using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.functions.tests.Builders;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.functions.tests
{
    public class TicketWorkflowFunctionsTests
    {
        [Fact]
        public void TicketWorkflowFunctions_()
        {
        }
        [Fact]
        public void TicketWorkflowFunctions_Constructor_Succeeds_With_Valid_Parameters()
        {
            var _ = new TicketWorkflowFunctionsBuilder().Build();
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void TicketWorkflowFunctions_Constructor_Guards_Against_NullParameters()
        {
            typeof(TicketWorkflowFunctions)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void TicketWorkflowFunctions_Constructor_Guards_Against_BadParameters()
        {
            typeof(TicketWorkflowFunctions)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task TicketWorkflowFunctions_ManualImport_Returns_Expected_Result()
        {
            const long ticketId = 4485;
            var createdTime = DateTime.Parse("10:10:57");

            var emailService = Substitute.For<IEmailService>();
            emailService
                .SendZendeskTicketCreatedEmail(ticketId, createdTime)
                .Returns(true);

            var dateTimeService = Substitute.For<IDateTimeService>();
            dateTimeService
                .UtcNow
                .Returns(createdTime);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Get,
                    $"https://test.com/SendTicketCreatedNotification?ticketId={ticketId}");

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(emailService, dateTimeService);
            
            var result = await functions.SendTicketCreatedNotification(request, functionContext);
            
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await result.Body.ReadAsString();
            body.Should().Be("Done!");

            await emailService
                .Received(1)
                .SendZendeskTicketCreatedEmail(Arg.Any<long>(), Arg.Any<DateTime>());
            await emailService
                .Received(1)
                .SendZendeskTicketCreatedEmail(ticketId, createdTime);
        }
    }
}
