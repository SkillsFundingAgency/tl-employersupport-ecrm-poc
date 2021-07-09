using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmODataApiClientBuilder
    {
        public IEcrmODataApiClient Build(
            HttpClient httpClient = null,
            IAuthenticationService authenticationService = null,
            ILogger<EcrmODataApiClient> logger = null)
        {
            logger ??= Substitute.For<ILogger<EcrmODataApiClient>>();
            httpClient ??= Substitute.For<HttpClient>();
            authenticationService ??= new AuthenticationServiceBuilder().Build();

            return new EcrmODataApiClient(httpClient, authenticationService, logger);
        }
    }
}
