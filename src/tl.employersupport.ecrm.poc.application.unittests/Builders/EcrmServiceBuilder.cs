﻿using Microsoft.Extensions.Logging;
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
            IEcrmXrmClient ecrmXrmClient = null,
            ILogger<EcrmService> logger = null)
        {
            ecrmApiClient ??= Substitute.For<IEcrmApiClient>();
            ecrmODataApiClient ??= Substitute.For<IEcrmODataApiClient>();
            ecrmXrmClient ??= Substitute.For<IEcrmXrmClient>();
            logger ??= Substitute.For<ILogger<EcrmService>>();
            
            return new EcrmService(
                ecrmApiClient, 
                ecrmODataApiClient, 
                ecrmXrmClient, 
                logger);
        }
    }
}
