using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmApiClientBuilder
    {
        public IEcrmApiClient Build(
            HttpClient httpClient = null,
            IOptions<EcrmConfiguration> configuration = null,
            ILogger < EcrmApiClient> logger = null)
        {
            logger ??= Substitute.For<ILogger<EcrmApiClient>>();
            httpClient ??= Substitute.For<HttpClient>();
            configuration ??= new Func<IOptions<EcrmConfiguration>>(() => {
                var config = Substitute.For<IOptions<EcrmConfiguration>>();
                config.Value.Returns(new EcrmConfiguration());
                return config;
            }).Invoke();

            return new EcrmApiClient(httpClient, configuration, logger);
        }
    }
}
