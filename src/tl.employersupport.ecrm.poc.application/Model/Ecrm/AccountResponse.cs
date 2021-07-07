using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Identity.Client;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    public class AccountResponse
    {
        [JsonPropertyName("value")]
        public IList<Account> Accounts { get; init; }
    }
}
