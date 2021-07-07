using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class AuthenticationServiceBuilder
    {
        public IAuthenticationService Build(
            string token = null)
        {
            token ??= "token";

            var authenticationService = Substitute.For<IAuthenticationService>();
            authenticationService.GetAccessToken().Returns(token);

            return authenticationService;
        }
    }
}
