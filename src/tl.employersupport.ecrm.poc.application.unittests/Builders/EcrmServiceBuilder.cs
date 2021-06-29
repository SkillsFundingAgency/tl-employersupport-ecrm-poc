using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmServiceBuilder
    {
        public IEcrmService Build(
            IEcrmApiClient ecrmApiClient = null,
            ILogger<EcrmService> logger = null)
        {
            ecrmApiClient ??= Substitute.For<IEcrmApiClient>();
            logger ??= Substitute.For<ILogger<EcrmService>>();
            
            return new EcrmService(ecrmApiClient, logger);
        }
    }
}
