using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Interfaces;

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
        public HttpResponseData HelloWorld([HttpTrigger(AuthorizationLevel.Function, "get", "post")]
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
        public async Task<HttpResponseData> SendTicketCreatedNotification([HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SendTicketCreatedNotification");

            try
            {
                logger.LogInformation($"{nameof(SendTicketCreatedNotification)} HTTP function called.");

                //var body = request.ReadAsStringAsync();
                var queryParameters = HttpUtility.ParseQueryString(request.Url.Query);

                var response = request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                if (long.TryParse(queryParameters.Get("ticketId"), out var ticketId))
                {
                    var sent = await _emailService.SendZendeskTicketCreatedEmail(ticketId, _dateTimeService.UtcNow);
                    await response.WriteStringAsync($"{(sent ? "Done!" : "Failed :(")}");
                }
                else
                {
                    await response.WriteStringAsync("Ticket Id not found in request.");
                }

                return response;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error in {nameof(SendTicketCreatedNotification)}. Internal Error Message {e}";
                logger.LogError(errorMessage);

                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("RandomTimeOut")]
        public HttpResponseData RandomTimeOut([HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HelloWorld");
            logger.LogInformation($"{nameof(RandomTimeOut)} HTTP trigger function called.");

            var queryParameters = HttpUtility.ParseQueryString(request.Url.Query);

            int.TryParse(queryParameters.Get("minTimeout"), out var minTimeout);
            int.TryParse(queryParameters.Get("maxTimeout"), out var maxTimeout);

            minTimeout = Math.Max(0, minTimeout);
            maxTimeout = Math.Max(maxTimeout, minTimeout);

            var random = new Random();
            var timeout = random.Next(minTimeout, maxTimeout) * 1000;

            logger.LogInformation($"{nameof(RandomTimeOut)} maxTimeout={minTimeout}, maxTimeout={maxTimeout} seconds.");
            logger.LogInformation($"{nameof(RandomTimeOut)} sleeping for {timeout}ms.");
            Thread.Sleep(timeout);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Success!");

            return response;
        }
    }
}
