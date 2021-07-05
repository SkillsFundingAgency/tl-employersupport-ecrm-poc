using Microsoft.Extensions.Logging;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Services;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmServiceBuilder
    {
        public IEcrmService Build(
            IEcrmApiClient ecrmApiClient = null,
            IEcrmODataApiClient ecrmODataApiClient = null,
            ILogger<EcrmService> logger = null)
        {
            ecrmApiClient ??= Substitute.For<IEcrmApiClient>();
            ecrmODataApiClient ??= Substitute.For<IEcrmODataApiClient>();
            logger ??= Substitute.For<ILogger<EcrmService>>();
            
            return new EcrmService(ecrmApiClient, ecrmODataApiClient, logger);
        }
    }
}
