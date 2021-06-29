using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public EcrmService(
            IEcrmApiClient ecrmApiClient,
            ILogger<EcrmService> logger)
        {
            _ecrmApiClient = ecrmApiClient ?? throw new ArgumentNullException(nameof(ecrmApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Employer> FindEmployer(EmployerSearchRequest searchRequest)
        {
            _logger.LogInformation($"Getting employer {searchRequest.CompanyName}");

            var employer = await _ecrmApiClient.GetEmployer(searchRequest);

            return employer;
        }
    }
}
