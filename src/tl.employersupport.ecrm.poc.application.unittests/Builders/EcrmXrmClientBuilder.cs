using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmXrmClientBuilder
    {
        public IEcrmXrmClient Build(
            IOrganizationService organizationService = null,
            ILogger<EcrmXrmClient> logger = null)
        {
            logger ??= Substitute.For<ILogger<EcrmXrmClient>>();
            organizationService ??= Substitute.For<IOrganizationService>();

            return new EcrmXrmClient(organizationService, logger);
        }
    }
}
