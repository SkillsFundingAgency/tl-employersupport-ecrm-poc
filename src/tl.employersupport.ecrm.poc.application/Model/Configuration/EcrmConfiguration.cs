
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace tl.employersupport.ecrm.poc.application.Model.Configuration
{
    public class EcrmConfiguration
    {
        public string ApiBaseUri { get; init; }
        public string ODataApiUri { get; init; }
        public string ODataApiVersion { get; init; }
        public string ApiKey { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string Tenant { get; init; }
    }
}
