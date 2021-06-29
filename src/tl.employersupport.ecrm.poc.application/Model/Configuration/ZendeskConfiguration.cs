
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace tl.employersupport.ecrm.poc.application.Model.Configuration
{
    public class ZendeskConfiguration
    {
        public AuthenticationScheme AuthenticationMethod { get; init; }
        public string ApiBaseUri { get; init; }
        public string ApiToken { get; init; }
        public string User { get; init; }
        public string Password { get; init; }
    }
}
