using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmApiClientBuilder
    {
        public IEcrmApiClient Build(
            HttpClient httpClient = null,
            ILogger<EcrmApiClient> logger = null)
        {
            logger ??= Substitute.For<ILogger<EcrmApiClient>>();
            httpClient ??= Substitute.For<HttpClient>();

            return new EcrmApiClient(httpClient, logger);
        }
    }
}
