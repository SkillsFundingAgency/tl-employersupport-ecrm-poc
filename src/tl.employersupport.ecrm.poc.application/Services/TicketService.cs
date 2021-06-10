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
                                    throw new ArgumentNullException(
                                        $"{nameof(zendeskConfiguration)}.{nameof(zendeskConfiguration.Value)}",
                                        "zendeskConfiguration configuration value must not be null");
        }

        public async Task<CombinedTicket> GetTicket(long ticketId)
        {
            Console.WriteLine($"Getting ticket {ticketId}");
            
            var ticketJson = await GetTicketJson(ticketId, Sideloads.GetTicketSideloads());
            var ticketCommentJson = await GetTicketCommentsJson(ticketId);
            var ticketAuditsJson = await GetTicketAuditsJson(ticketId);

            _logger.LogInformation($"Ticket json: \n{ticketJson.PrettifyJsonString()}");
            _logger.LogInformation($"Ticket comments json: \n{ticketCommentJson.PrettifyJsonString()}");
            _logger.LogInformation($"Ticket audits json: \n{ticketAuditsJson.PrettifyJsonString()}");

            //TODO: Change prettifier to take doc as well as string, then use the following to convert all:
            //var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            var zendeskTicketResponse = ticketJson.DeserializeZendeskTicketResponse();
            var zendeskTicket = ticketJson.DeserializeZendeskTicket();
            var comments = ticketCommentJson.DeserializeZendeskComments();
            var audits = ticketAuditsJson.DeserializeZendeskAudits();

            //TODO: Get the attachments

            return new CombinedTicket
            {
                Id = zendeskTicket.Id,
                Ticket = zendeskTicketResponse.Ticket,
                Users = zendeskTicketResponse.Users,
                Groups = zendeskTicketResponse.Groups,
                Organizations = zendeskTicketResponse.Organizations,
                Comments = comments,
                Audits = audits,
            };
        }

        private async Task<string> GetTicketJson(long ticketId, string sideloads)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await GetJson($"tickets/{ticketId}.json{requestQueryString}");
        }

        private async Task<string> GetTicketCommentsJson(long ticketId) => 
            await GetJson($"tickets/{ticketId}/comments.json");

        private async Task<string> GetTicketAuditsJson(long ticketId) => 
            await GetJson($"tickets/{ticketId}/audits.json");

        private async Task<string> GetJson(string requestUriFragment)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(TicketService));

            _logger.LogInformation($"Calling Zendesk Support API {httpClient.BaseAddress} endpoint {requestUriFragment}");

            var response = await httpClient.GetAsync(requestUriFragment);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
