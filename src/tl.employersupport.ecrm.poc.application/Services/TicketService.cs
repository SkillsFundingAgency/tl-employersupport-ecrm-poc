using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public async Task<Ticket> GetTicket(int id)
        {
            Console.WriteLine($"Getting ticket {id}");

            _logger.LogInformation($"TicketService:: API URL = {_zendeskConfiguration.ApiBaseUri}");
            _logger.LogInformation($"TicketService:: API Token = {_zendeskConfiguration.ApiToken}");

            return new Ticket
            {
                Id = id
            };
        }
    }
}
