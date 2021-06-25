using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class ZendeskApiClientBuilder
    {
        public IZendeskApiClient Build(
            HttpClient httpClient = null,
            ILogger<ZendeskApiClient> logger = null)
        {
            logger ??= Substitute.For<ILogger<ZendeskApiClient>>();

            httpClient ??= Substitute.For<HttpClient>();

            return new ZendeskApiClient(httpClient, logger);
        }
    }
}
