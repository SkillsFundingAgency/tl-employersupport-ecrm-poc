using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using tl.employersupport.ecrm.poc.application.HttpHandlers;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.tests.Extensions;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.tests.HttpHandlers
{
    public class ZendeskApiTokenMessageHandlerTests : DelegatingHandler
    {
        [Fact]
        public void ZendeskApiTokenMessageHandler_Constructor_Guards_Against_NullParameters()
        {
            typeof(ZendeskApiTokenMessageHandler)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void ZendeskApiTokenMessageHandler_Constructor_Guards_Against_Null_Configuration()
        {
            var settings = Substitute.For<IOptions<ZendeskConfiguration>>();
            settings.Value.ReturnsNull();

            Assert.Throws<ArgumentNullException>(
                "settings",
                () => new ZendeskApiTokenMessageHandler(settings)
            );
        }

        [Fact]
        public async Task ZendeskApiTokenMessageHandler_Adds_Authorization_Header_With_Api_Token()
        {
            const string user = "test.user@test.zendesk.com";
            const string apiToken = "46c5c7a37b6d45d081b766a440828fd1";
            var expectedAuthenticationHeader = "Basic " +
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{user}/token:{apiToken}"));

            var settings = new ZendeskConfiguration
            {
                User = user,
                ApiToken = apiToken,
                AuthenticationMethod = AuthenticationScheme.BasicWithApiToken
            };

            var request = new HttpRequestMessage(HttpMethod.Get, "https://test.com/");

            var handler = CreateHandler(request, settings);
            var response = await new HttpMessageInvoker(handler)
                .SendAsync(
                    request, 
                    new CancellationToken());

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var header = request.Headers.GetValues("Authorization").FirstOrDefault();
            header.Should().NotBeNull();
            header.Should().Be(expectedAuthenticationHeader);
        }

        private ZendeskApiTokenMessageHandler CreateHandler(HttpRequestMessage request, ZendeskConfiguration settings)
        {
            var options = Substitute.For<IOptions<ZendeskConfiguration>>();
            options.Value.Returns(settings);

            var innerHandlerMock = Substitute.For<DelegatingHandler>();

            innerHandlerMock
                .GetType()
                .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)
                !.Invoke(innerHandlerMock, new object[]
                {
                    request,
                    Arg.Any<CancellationToken>()
                })
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            var handler = new ZendeskApiTokenMessageHandler(options)
            {
                InnerHandler = innerHandlerMock
            };

            return handler;
        }

        [Fact]
        public async Task ZendeskApiTokenMessageHandler_Adds_Authorization_Header_With_User_And_Password()
        {
            const string user = "test.user@test.zendesk.com";
            const string password = "b0bby!";
            var expectedAuthenticationHeader = "Basic " +
                                               Convert.ToBase64String(
                                                   Encoding.ASCII.GetBytes(
                                                       $"{user}:{password}"));

            var settings = new ZendeskConfiguration
            {
                User = user,
                Password = password,
                AuthenticationMethod = AuthenticationScheme.BasicWithUserPassword
            };

            var request = new HttpRequestMessage(HttpMethod.Get, "https://test.com/");

            var handler = CreateHandler(request, settings);
            var response = await new HttpMessageInvoker(handler)
                .SendAsync(
                    request,
                    new CancellationToken());

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var header = request.Headers.GetValues("Authorization").FirstOrDefault();
            header.Should().NotBeNull();
            header.Should().Be(expectedAuthenticationHeader);
        }

        [Fact]
        public async Task ZendeskApiTokenMessageHandler_Throws_Exception_For_Missing_Authentication_Scheme()
        {
            var settings = new ZendeskConfiguration
            {
                User = null,
                Password = null,
                AuthenticationMethod = (AuthenticationScheme)999
            };

            var request = new HttpRequestMessage(HttpMethod.Get, "https://test.com/");

            var handler = CreateHandler(request, settings);

            await Assert.ThrowsAsync<NotSupportedException>(() => 
                new HttpMessageInvoker(handler)
                .SendAsync(
                    request,
                    new CancellationToken()));
        }
    }
}
