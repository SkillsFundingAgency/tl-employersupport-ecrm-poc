using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tl.employersupport.ecrm.poc.application.Model.Configuration;

namespace tl.employersupport.ecrm.poc.application.HttpHandlers
{
    public class ZendeskApiTokenMessageHandler : DelegatingHandler
    {
        private readonly ZendeskConfiguration _settings;

        public ZendeskApiTokenMessageHandler(
            IOptions<ZendeskConfiguration> settings)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            switch (_settings.AuthenticationMethod)
            {
                case AuthenticationScheme.BasicWithUserPassword:
                    {
                        //Basic auth with user/password - email_address:password and this must be base64-encoded
                        var userAuthenticationString = $"{_settings.User}:{_settings.Password}";
                        var encodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(userAuthenticationString));
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("Basic", encodedAuthenticationString);
                        break;
                    }
                case AuthenticationScheme.BasicWithApiToken:
                    {
                        //Basic auth with token - email_address/token:api_token and this must be base64-encoded
                        var tokenAuthenticationString = $"{_settings.User}/token:{_settings.ApiToken}";
                        var encodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(tokenAuthenticationString));
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("Basic", encodedAuthenticationString);
                        break;
                    }
                default:
                    throw new NotSupportedException(
                        $"Invalid Zendesk authentication scheme '{_settings.AuthenticationMethod}'");
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
