using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.functions.unittests.Builders;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;
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
        public async Task TicketWorkflowFunctions_RetrieveEmployerContactTicket_Via_Get_Returns_Expected_Result()
        {
            const long ticketId = 4485;

            var ticket = new EmployerContactTicketBuilder()
                .WithDefaultValues()
                .Build();

            var ticketService = Substitute.For<ITicketService>();
            ticketService
                .GetEmployerContactTicket(ticketId)
                .Returns(ticket);

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Get,
                    $"https://test.com/RetrieveEmployerContactTicket?ticketId={ticketId}");

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(ticketService: ticketService);

            var result = await functions.RetrieveEmployerContactTicket(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            result.Body.Should().NotBeNull();
            
            result.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(result.Body);
            var responseJson = await reader.ReadToEndAsync();

            var deserializedTicket = JsonSerializer
                .Deserialize<EmployerContactTicket>(responseJson,
                    JsonExtensions.DefaultJsonSerializerOptions);

            CheckEmployerContactTicket(deserializedTicket, ticketId);

            await ticketService
                .Received(1)
                .GetEmployerContactTicket(ticketId);
        }

        [Fact]
        public async Task TicketWorkflowFunctions_RetrieveEmployerContactTicket_Via_Post_Returns_Expected_Result()
        {
            const long ticketId = 4485;

            var ticket = new EmployerContactTicketBuilder()
                .WithDefaultValues()
                .Build();

            var ticketService = Substitute.For<ITicketService>();
            ticketService
                .GetEmployerContactTicket(ticketId)
                .Returns(ticket);

            var notification = new NotifyTicket
            {
                Id = ticketId
            };

            var requestJson = JsonSerializer.Serialize(notification,
                JsonExtensions.DefaultJsonSerializerOptions);

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/RetrieveEmployerContactTicket",
                    requestJson);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(ticketService: ticketService);

            var result = await functions.RetrieveEmployerContactTicket(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            result.Body.Should().NotBeNull();

            result.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(result.Body);
            var responseJson = await reader.ReadToEndAsync();

            var deserializedTicket = JsonSerializer
                .Deserialize<EmployerContactTicket>(responseJson,
                    JsonExtensions.DefaultJsonSerializerOptions);

            CheckEmployerContactTicket(deserializedTicket, ticketId);

            await ticketService
                .Received(1)
                .GetEmployerContactTicket(ticketId);
        }

        [Fact]
        public async Task TicketWorkflowFunctions_RetrieveEmployerContactTicket_With_No_Data_Via_Get_Returns_Bad_Request_Result()
        {
            const long ticketId = 4485;

            var ticket = new EmployerContactTicketBuilder()
                .WithDefaultValues()
                .Build();

            var ticketService = Substitute.For<ITicketService>();
            ticketService
                .GetEmployerContactTicket(ticketId)
                .Returns(ticket);
            
            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/RetrieveEmployerContactTicket");

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(ticketService: ticketService);

            var result = await functions.RetrieveEmployerContactTicket(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await ticketService
                .DidNotReceive()
                .GetEmployerContactTicket(Arg.Any<long>());
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

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Get,
                    $"https://test.com/SendTicketCreatedNotification?ticketId={ticketId}");

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

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
            
            var requestJson = JsonSerializer.Serialize(notification,
                JsonExtensions.DefaultJsonSerializerOptions);

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/SendTicketCreatedNotification",
                    requestJson);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

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
            var emailService = Substitute.For<IEmailService>();

            var dateTimeService = Substitute.For<IDateTimeService>();

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/SendTicketCreatedNotification");

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

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

            var requestJson = JsonSerializer.Serialize(modifyTagsRequest,
                JsonExtensions.DefaultJsonSerializerOptions);

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/ModifyZendeskTicketTags",
                    requestJson);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
            
            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(ticketService: ticketService);

            var result = await functions.ModifyZendeskTicketTags(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            await ticketService
                .Received(1)
                .GetTicketTags(ticketId);
        }

        [Fact]
        public async Task TicketWorkflowFunctions_QueueTicketRequest_Returns_Expected_Result()
        {
            const long ticketId = 4485;

            var notification = new NotifyTicket
            {
                Id = ticketId
            };

            var requestJson = JsonSerializer.Serialize(notification,
                JsonExtensions.DefaultJsonSerializerOptions);

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/QueueTicketRequest",
                    requestJson);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build();

            var result = await functions.QueueTicketRequest(request, functionContext);

            result.Should().Be(requestJson);
        }

        [Fact]
        public async Task TicketWorkflowFunctions_SearchEcrmEmployer_Returns_Expected_Result()
        {
            var searchRequest =  new EmployerSearchRequestBuilder()
                .Build();

            var employer = new EmployerBuilder()
                .Build();

            var ecrmService = Substitute.For<IEcrmService>();
            ecrmService
                .FindEmployer(Arg.Is<EmployerSearchRequest>(
                    s => s.CompanyName == searchRequest.CompanyName))
                .Returns(employer);

            var requestJson = JsonSerializer.Serialize(searchRequest,
                JsonExtensions.DefaultJsonSerializerOptions);

            var request = FunctionObjectsBuilder
                .BuildHttpRequestData(
                    HttpMethod.Post,
                    "https://test.com/FindEmployer",
                    requestJson);

            var functionContext = FunctionObjectsBuilder.BuildFunctionContext();

            var functions = new TicketWorkflowFunctionsBuilder()
                .Build(ecrmService: ecrmService);

            var result = await functions.SearchEcrmEmployer(request, functionContext);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            result.Body.Should().NotBeNull();

            result.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(result.Body);
            var responseJson = await reader.ReadToEndAsync();

            var deserializedEmployer = JsonSerializer
                .Deserialize<Employer>(responseJson,
                    JsonExtensions.DefaultJsonSerializerOptions);
            
            CheckEmployer(deserializedEmployer);

            await ecrmService
                .Received(1)
                .FindEmployer(Arg.Is<EmployerSearchRequest>(
                    s => s.CompanyName == searchRequest.CompanyName));
        }

        private static void CheckEmployerContactTicket(EmployerContactTicket ticket, long ticketId)
        {
            ticket.Should().NotBeNull();
            ticket.Id.Should().Be(ticketId);
            ticket.CreatedAt.Should()
                .Be(DateTime.Parse("2021-06-23 14:40"));
            ticket.UpdatedAt.Should()
                .Be(DateTime.Parse("2021-06-24 12:19"));

            ticket.RequestedBy.Should().NotBeNull();
            ticket.RequestedBy.Id.Should().Be(12855186);
            ticket.RequestedBy.Name.Should().Be("Bob Bobs");
            ticket.RequestedBy.Email.Should().Be("bob.bobs@bobbobs.com");

            ticket.Organisation.Should().NotBeNull();
            ticket.Organisation.Id.Should().Be(456876);
            ticket.Organisation.Name.Should().Be("Large Corporation Limited");
        }

        private static void CheckEmployer(Employer employer)
        {
            employer.Should().NotBeNull();
            employer.AccountId.Should().Be(Guid.Parse("461082b5-d2ea-475b-bf85-2417e650aa68"));
            employer.CompanyName.Should().Be("Fake Company");
        }
    }
}
