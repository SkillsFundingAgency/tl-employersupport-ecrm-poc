using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    public class AccountResponse
    {
        [JsonPropertyName("value")]
        public IList<Account> Accounts { get; init; }
    }
}
