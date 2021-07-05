using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.functions
{
    public class TicketWorkflowFunctions
    {
        private readonly IEmailService _emailService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ITicketService _ticketService;
        private readonly IEcrmService _ecrmService;

        public TicketWorkflowFunctions(
            IDateTimeService dateTimeService,
            IEmailService emailService,
            ITicketService ticketService,
            IEcrmService ecrmService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _ecrmService = ecrmService ?? throw new ArgumentNullException(nameof(ecrmService));
        }

        [Function("HelloWorld")]
        // ReSharper disable once UnusedMember.Global
        public HttpResponseData HelloWorld(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HelloWorld");
            logger.LogInformation("C# 'Hello World' HTTP trigger function processed a request.");

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("SendTicketCreatedNotification")]
        public async Task<HttpResponseData> SendTicketCreatedNotification(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SendTicketCreatedNotification");

            try
            {
                logger.LogInformation($"{nameof(SendTicketCreatedNotification)} HTTP function called via {request.Method}.");

                var ticketId = await ReadTicketIdFromRequestData(request, logger);

                if (ticketId <= 0)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                }

                await _emailService.SendZendeskTicketCreatedEmail(ticketId, _dateTimeService.UtcNow);

                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(SendTicketCreatedNotification)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("RetrieveEmployerContactTicket")]
        public async Task<HttpResponseData> RetrieveEmployerContactTicket(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("RetrieveEmployerContactTicket");

            try
            {
                logger.LogInformation($"{nameof(SendTicketCreatedNotification)} HTTP function called via {request.Method}.");

                var ticketId = await ReadTicketIdFromRequestData(request, logger);

                if (ticketId <= 0)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var ticket = await _ticketService.GetEmployerContactTicket(ticketId);

                if (ticket == null)
                {
                    return request.CreateResponse(HttpStatusCode.NotFound);
                }

                var response = request.CreateResponse(HttpStatusCode.OK);
                //response.Headers.Add("Content-Type", "application/json");

                //var json = JsonSerializer.Serialize(ticket,
                //    JsonExtensions.DefaultJsonSerializerOptions);
                //await response.WriteStringAsync(json);

                await response.WriteAsJsonAsync(ticket, "application/json");

                return response;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(SendTicketCreatedNotification)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("ModifyZendeskTicketTags")]
        public async Task<HttpResponseData> ModifyZendeskTicketTags(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("ModifyZendeskTicketTags");

            try
            {
                logger.LogInformation($"{nameof(ModifyZendeskTicketTags)} HTTP function called via {request.Method}.");

                var body = await request.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    logger.LogError("Request body was empty");
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var tagsMessage = JsonSerializer
                    .Deserialize<ModifyTagsMessage>(body,
                        JsonExtensions.DefaultJsonSerializerOptions);

                var ticketId = tagsMessage?.TicketId ?? 0;

                //Validation
                if (ticketId <= 0)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                }

                //Get ticket so we can pull out latest updated date
                var existingTags = await _ticketService.GetTicketTags(ticketId);

                //TODO: Define error handing when ticket not found
                if (existingTags != null)
                {
                    //Merge any new tags in
                    var hasNewTags = false;
                    foreach (var tag in tagsMessage!.Tags
                        .Where(t => !existingTags.Tags.Contains(t)))
                    {
                        existingTags.Tags.Add(tag);
                        hasNewTags = true;
                    }

                    if (hasNewTags)
                    {
                        var safeTags = new SafeTags
                        {
                            Tags = existingTags.Tags,
                            UpdatedStamp = existingTags.UpdatedStamp
                        };

                        await _ticketService.ModifyTags(ticketId, safeTags);
                    }
                }

                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(ModifyZendeskTicketTags)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("RandomTimeOut")]
        // ReSharper disable once UnusedMember.Global
        public async Task<HttpResponseData> RandomTimeOut(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("RandomTimeOut");
            try
            {
                logger.LogInformation($"{nameof(RandomTimeOut)} HTTP trigger function called.");

                var queryParameters = HttpUtility.ParseQueryString(request.Url.Query);

                var minTimeout = int.Parse(queryParameters.Get("minTimeout") ?? string.Empty);
                var maxTimeout = int.Parse(queryParameters.Get("maxTimeout") ?? string.Empty);

                minTimeout = Math.Max(0, minTimeout);
                maxTimeout = Math.Max(maxTimeout, minTimeout);

                var random = new Random();
                var timeout = random.Next(minTimeout, maxTimeout);

                logger.LogInformation($"{nameof(RandomTimeOut)} maxTimeout={minTimeout}, maxTimeout={maxTimeout} seconds.");
                logger.LogInformation($"{nameof(RandomTimeOut)} sleeping for {timeout} seconds.");

                await Task.Delay(TimeSpan.FromSeconds(timeout));

                var response = request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                await response.WriteStringAsync("Success!");

                return response;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(RandomTimeOut)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("CheckEcrmHeartbeat")]
        public async Task<HttpResponseData> CheckEcrmHeartbeat(
            [HttpTrigger(AuthorizationLevel.Function, "get")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CheckEcrmHeartbeat");

            try
            {
                logger.LogInformation($"{nameof(CheckEcrmHeartbeat)} HTTP function called via {request.Method}.");

                var isOk = await _ecrmService.Heartbeat();

                var response = isOk ?
                    request.CreateResponse(HttpStatusCode.OK)
                    : request.CreateResponse(HttpStatusCode.NotFound);

                return response;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(SendTicketCreatedNotification)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("SearchEcrmEmployer")]
        public async Task<HttpResponseData> SearchEcrmEmployer(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SearchEcrmEmployer");

            try
            {
                logger.LogInformation($"{nameof(SendTicketCreatedNotification)} HTTP function called via {request.Method}.");

                var employerSearchRequest = await ReadEmployerSearchRequestFromRequestData(request, logger);

                if (employerSearchRequest == null)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var employer = await _ecrmService.FindEmployer(employerSearchRequest);

                if (employer == null)
                {
                    return request.CreateResponse(HttpStatusCode.NotFound);
                }

                var response = request.CreateResponse(HttpStatusCode.OK);

                await response.WriteAsJsonAsync(employer, "application/json");

                return response;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(SendTicketCreatedNotification)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("QueueTicketRequest")]
        [ServiceBusOutput("ticket-queue", Connection = "ServiceBusConnectionString", EntityType = EntityType.Queue)]
        public async Task<string> QueueTicketRequest(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("QueueTicketRequest");

            try
            {
                logger.LogInformation($"{nameof(SendTicketCreatedNotification)} HTTP function called via {request.Method}.");

                return await request.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(SendTicketCreatedNotification)}. Internal Error Message {e}";
                logger.LogError(errorMessage);
                throw;
            }
        }

        private async Task<long> ReadTicketIdFromRequestData(HttpRequestData request, ILogger logger)
        {
            var ticketId = 0L;

            var body = await request.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
            {
                logger.LogDebug($"Body was read successfully: {body}");
                try
                {
                    var notification = JsonSerializer
                        .Deserialize<NotifyTicket>(body,
                            JsonExtensions.DefaultJsonSerializerOptions);

                    ticketId = notification?.Id ?? 0;
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to deserialize request body {ex}", ex);
                }
            }

            if (ticketId == 0)
            {
                var queryParameters = HttpUtility.ParseQueryString(request.Url.Query);
                if (!long.TryParse(queryParameters.Get("ticketId"), out ticketId))
                {
                    logger.LogError("Failed to read ticket id from query string");
                }
            }

            return ticketId;
        }

        private async Task<EmployerSearchRequest> ReadEmployerSearchRequestFromRequestData(HttpRequestData request, ILogger logger)
        {
            var body = await request.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
            {
                logger.LogDebug($"Body was read successfully: {body}");
                try
                {
                    var employerSearchRequest = JsonSerializer
                        .Deserialize<EmployerSearchRequest>(body,
                            JsonExtensions.DefaultJsonSerializerOptions);

                    return employerSearchRequest;
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to deserialize request body {ex}", ex);
                }
            }

            return null;
        }

    }
}
