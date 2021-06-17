using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.functions
{
    public class TicketWorkflowFunctions
    {
        private readonly IEmailService _emailService;
        private readonly IDateTimeService _dateTimeService;

        public TicketWorkflowFunctions(
            IEmailService emailService,
            IDateTimeService dateTimeService = null)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
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

                var ticketId = 0L;

                var body = await request.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    logger.LogInformation("Body was empty");
                }
                else
                {
                    logger.LogInformation($"Body was read successfully: {body}");
                    try
                    {
                        //Attempt to deserialize
                        var notification = JsonSerializer
                            .Deserialize<NotifyTicket>(
                                body,
                                new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                });

                        ticketId = notification?.Id ?? 0;
                        logger.LogInformation($"After deserialization ticket id is {ticketId}");
                        logger.LogInformation($"After deserialization notification is null = {(notification is null)}");
                        logger.LogInformation($"After deserialization notification is'{notification}'");
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

                int.TryParse(queryParameters.Get("minTimeout"), out var minTimeout);
                int.TryParse(queryParameters.Get("maxTimeout"), out var maxTimeout);

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
    }
}
