using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class EcrmService : IEcrmService
    {
        private readonly ILogger _logger;
        private readonly IEcrmApiClient _ecrmApiClient;
        private readonly IEcrmODataApiClient _ecrmODataApiClient;

        public EcrmService(
            IEcrmApiClient ecrmApiClient,
            IEcrmODataApiClient ecrmODataApiClient,
            ILogger<EcrmService> logger)
        {
            _ecrmApiClient = ecrmApiClient ?? throw new ArgumentNullException(nameof(ecrmApiClient));
            _ecrmODataApiClient = ecrmODataApiClient;
            _ecrmODataApiClient = ecrmODataApiClient ?? throw new ArgumentNullException(nameof(ecrmODataApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Employer> FindEmployer(EmployerSearchRequest searchRequest)
        {
            _logger.LogInformation($"Getting employer {searchRequest.CompanyName}");

            var employer = await _ecrmApiClient.GetEmployer(searchRequest);

            return employer;
        }

        public async Task<bool> Heartbeat()
        {
            return await _ecrmApiClient.GetHeartbeat();
        }

        public async Task<WhoAmIResponse> WhoAmI()
        {
            return await _ecrmODataApiClient.GetWhoAmI();
        }

        public async Task<Guid> CreateAccount(Account account)
        {
            return await _ecrmODataApiClient.CreateAccount(account);
        }

        public async Task<Account> GetAccount(Guid accountId)
        {
            return await _ecrmODataApiClient.GetAccount(accountId);
        }
    }
}
