using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ZendeskConfiguration _zendeskConfiguration;

        public TicketService(
            IHttpClientFactory httpClientFactory,
            ILogger<TicketService> logger,
            IOptions<ZendeskConfiguration> zendeskConfiguration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Note: check this last otherwise unit tests checking null ctor args will fail
            if (zendeskConfiguration is null)
                throw new ArgumentNullException(nameof(zendeskConfiguration));

            _zendeskConfiguration = zendeskConfiguration.Value ??
                                    throw new ArgumentNullException($"{nameof(zendeskConfiguration)}.{nameof(zendeskConfiguration.Value)}",
                                        "zendeskConfiguration configuration value must not be null");
        }

        public async Task<Ticket> GetTicket(int ticketId)
        {
            Console.WriteLine($"Getting ticket {ticketId}");

            _logger.LogInformation($"TicketService:: API URL = {_zendeskConfiguration.ApiBaseUri}");
            _logger.LogInformation($"TicketService:: API Token = {_zendeskConfiguration.ApiToken}");

            //https://developer.zendesk.com/rest_api/docs/support/tickets#show-ticket
            //GET /api/v2/tickets/{ticket_id}
            var httpClient = _httpClientFactory.CreateClient(nameof(TicketService));
            var requestUri = $"tickets/{ticketId}";

            _logger.LogInformation($"Calling Zendesk Support API {httpClient.BaseAddress} endpoint {requestUri}");

            var response = await httpClient.GetAsync(requestUri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var prettifiedJsonResponse = jsonResponse. PrettifyJsonString();

            _logger.LogInformation("Zendesk Support API succeeded. Result was:");
            _logger.LogInformation(prettifiedJsonResponse);

            return new Ticket
            {
                Id = ticketId
            };
        }
    }
}
