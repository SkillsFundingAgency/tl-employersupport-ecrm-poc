using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.functions.unittests.Builders;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.tests.common.Extensions;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.functions.unittests
{
    public class TicketWorkflowFunctionsTests
    {
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
        public async Task TicketWorkflowFunctions_SendTicketCreatedNotification_Via_Get_Returns_Expected_Result()
        {
            const long ticketId = 4485;
            var createdTime = DateTime.Parse("2021-06-15 10:10:57");

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
                .Build(dateTimeService, emailService);
            
            var result = await functions.SendTicketCreatedNotification(request, functionContext);
            
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            await emailService
                .Received(1)
                .SendZendeskTicketCreatedEmail(ticketId, createdTime);
        }

        [Fact]
        public async Task TicketWorkflowFunctions_SendTicketCreatedNotification_Via_Post_Returns_Expected_Result()
        {
            const long ticketId = 4485;
            var createdTime = DateTime.Parse("2021-06-15 10:10:57");

            var emailService = Substitute.For<IEmailService>();
            emailService
                .SendZendeskTicketCreatedEmail(ticketId, createdTime)
                .Returns(true);

            var dateTimeService = Substitute.For<IDateTimeService>();
            dateTimeService
                .UtcNow
                .Returns(createdTime);

            var notification = new NotifyTicket
            {
                Id = ticketId
            };

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

            var json = JsonSerializer.Serialize(notification,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    $"https://test.com/SendTicketCreatedNotification",
                    json);

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(dateTimeService, emailService);

            var result = await functions.SendTicketCreatedNotification(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            await emailService
                .Received(1)
                .SendZendeskTicketCreatedEmail(ticketId, createdTime);
        }

        [Fact]
        public async Task TicketWorkflowFunctions_SendTicketCreatedNotification_With_No_Data_Via_Get_Returns_Bad_Request_Result()
        {
            const long ticketId = 4485;
            var createdTime = DateTime.Parse("2021-06-15 10:10:57");

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
                    HttpMethod.Post,
                    $"https://test.com/SendTicketCreatedNotification");

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(dateTimeService, emailService);

            var result = await functions.SendTicketCreatedNotification(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await emailService
                .DidNotReceive()
                .SendZendeskTicketCreatedEmail(Arg.Any<long>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task TicketWorkflowFunctions_ModifyTags_Returns_Expected_Result()
        {
            const long ticketId = 4485;
            var updatedTime = DateTime.Parse("2021-06-15 10:10:57");

            var existingTags = new List<string>()
            {
                "1-9__micro_",
                "customer",
                "i_m_ready_to_offer_an_industry_placement",
                "tlevels-email",
                "tlevels_approved"
            };

            var ticketService = Substitute.For<ITicketService>();
            ticketService
                .GetTicketTags(ticketId)
                .Returns(new SafeTags {
                    Tags = existingTags, 
                    UpdatedStamp = new DateTimeOffset(updatedTime)
                });

            var modifyTagsRequest = new ModifyTagsMessage
            {
                TicketId = ticketId,
                Tags = existingTags
            };

            var json = JsonSerializer.Serialize(modifyTagsRequest,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    $"https://test.com/ModifyZendeskTicketTags",
                    json);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
            
            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(ticketService: ticketService);

            var result = await functions.ModifyZendeskTicketTags(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            await ticketService
                .Received(1)
                .GetTicketTags(ticketId);
        }
    }
}
