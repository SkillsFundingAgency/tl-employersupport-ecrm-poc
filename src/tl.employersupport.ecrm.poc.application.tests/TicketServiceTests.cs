using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.application.tests.Builders;
using tl.employersupport.ecrm.poc.application.tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace tl.employersupport.ecrm.poc.application.tests
{
    public class TicketServiceTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper _output;


        public TicketServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void TicketService_Constructor_Succeeds_With_Valid_Parameters()
        {
            var config = Substitute.For<IOptions<ZendeskConfiguration>>();
            config.Value.Returns(new ZendeskConfiguration());

            var _ = new TicketServiceBuilder().Build(zendeskConfiguration: config);
            //Test passes if no exceptions thrown
        }

        [Fact]
        public void TicketService_Constructor_Guards_Against_NullParameters()
        {
            typeof(TicketService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void TicketService_Constructor_Guards_Against_BadParameters()
        {
            typeof(TicketService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public void TicketService_Constructor_Guards_Against_Bad_Configuration()
        {
            Assert.Throws<ArgumentNullException>(
                "zendeskConfiguration.Value",
                () =>
                    new TicketService(
                        Substitute.For<IHttpClientFactory>(),
                        Substitute.For<ILogger<TicketService>>(),
                        Substitute.For<IOptions<ZendeskConfiguration>>())
            );
        }
    }
}
