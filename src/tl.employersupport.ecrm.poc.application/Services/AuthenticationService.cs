using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<string> _scopes;
        private readonly IConfidentialClientApplication _app;

        public AuthenticationService(AuthenticationConfiguration authentication)
        {
            //Can this be done at startup using https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-provide-httpclient
            _app = ConfidentialClientApplicationBuilder
                //.CreateWithApplicationOptions(new ConfidentialClientApplicationOptions { ClientId = ....})
                .Create(authentication.ClientId)
                .WithClientSecret(authentication.ClientSecret)
                .WithTenantId(authentication.Tenant)
                .Build();

            _scopes = new List<string> { $"{authentication.Audience}/.default" };
        }

        public async Task<string> GetAccessToken()
        {
            var authenticationResult = await _app.AcquireTokenForClient(_scopes)
                .ExecuteAsync();
            return authenticationResult.AccessToken;
        }
    }
}
