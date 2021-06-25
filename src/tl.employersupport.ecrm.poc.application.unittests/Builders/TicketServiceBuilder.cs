using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class TicketServiceBuilder
    {
        public ITicketService Build(
            IZendeskApiClient zendeskApiClient = null,
            ILogger<TicketService> logger = null,
            IOptions<ZendeskConfiguration> configuration = null)
        {
            logger ??= Substitute.For<ILogger<TicketService>>();

            zendeskApiClient ??= Substitute.For<IZendeskApiClient>();

            //Hard to read code - production version should use an if structure. It's just creating an empty config if null is passed in.
            configuration ??= new Func<IOptions<ZendeskConfiguration>>(() =>  {
                var config = Substitute.For<IOptions<ZendeskConfiguration>>();
                config.Value.Returns(new ZendeskConfiguration());
                return config;
            }).Invoke();

            return new TicketService(zendeskApiClient, logger, configuration);
        }
    }
}
