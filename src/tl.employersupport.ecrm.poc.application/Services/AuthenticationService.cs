using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.Services
{
    //https://stackoverflow.com/questions/38494279/how-do-i-get-an-oauth-2-0-authentication-token-in-c-sharp
    //https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki#conceptual-documentation
    //  https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-credential-flows
    //  https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/AcquireTokenSilentAsync-using-a-cached-token
    //
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<string> _scopes;
        //private readonly IConfidentialClientApplication _app;

        //public AuthenticationService(AuthenticationConfiguration authentication)
        //{
        //    _app = ConfidentialClientApplicationBuilder
        //        .Create(authentication.ClientId)
        //        .WithClientSecret(authentication.ClientSecret)
        //        .WithAuthority(authentication.Authority)
        //        .Build();

        //    _scopes = new List<string> { $"{authentication.Audience}/.default" };
        //}

        public async Task<string> GetAccessToken()
        {
            //var authenticationResult = await _app.AcquireTokenForClient(_scopes)
            //    .ExecuteAsync();
            //return authenticationResult.AccessToken;
            return null;
        }
    }
}
