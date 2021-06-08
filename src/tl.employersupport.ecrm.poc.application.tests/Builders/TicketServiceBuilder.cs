using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Services;

namespace tl.employersupport.ecrm.poc.application.tests.Builders
{
    public class TicketServiceBuilder
    {
        //private const string ZendeskApiBaseUri = "https://zendesk.test.api/v2/";
        //private const string ActionUriFragment = "getticket";
        
        public ITicketService Build(
            IHttpClientFactory httpClientFactory = null,
            ILogger<TicketService> logger = null,
            IOptions<ZendeskConfiguration> zendeskConfiguration = null)
        {
            logger ??= Substitute.For<ILogger<TicketService>>();

            //Hard to read code - probably needs to be replaced with an if structure
            zendeskConfiguration ??= new Func<IOptions<ZendeskConfiguration>>(() =>  {
                var config = Substitute.For<IOptions<ZendeskConfiguration>>();
                config.Value.Returns(new ZendeskConfiguration());
                return config;
            }).Invoke();

            httpClientFactory ??= Substitute.For<IHttpClientFactory>();
            //if (httpClientFactory is null)
            //{
                //var dataBuilder = new TicketJsonBuilder();

                //var httpClient =
                //    new TestHttpClientFactory()
                //        .CreateHttpClient(
                //            new Uri(new Uri(ZendeskApiBaseUri), ActionUriFragment),
                //            dataBuilder.Build());
                //httpClient.BaseAddress = new Uri(ZendeskApiBaseUri);

                //var httpClientFactory = Substitute.For<IHttpClientFactory>();
                //httpClientFactory
                //    .CreateClient(nameof(TicketService))
                //    .Returns(httpClient);
            //}

            return new TicketService(httpClientFactory, logger, zendeskConfiguration);
        }
    }
}
