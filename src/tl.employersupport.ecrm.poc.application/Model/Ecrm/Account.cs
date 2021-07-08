using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class Account
    {
        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("accountid")]
        public Guid? AccountId { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }
        
        [JsonPropertyName("address1_primarycontactname")]
        public string AddressPrimaryContact { get; set; }

        [JsonPropertyName("address1_line1")]
        public string AddressLine { get; set; }

        [JsonPropertyName("address1_postalcode")]
        public string Postcode { get; set; }

        [JsonPropertyName("address1_city")]
        public string AddressCity { get; set; }

        //[JsonPropertyName("customertypecode")]
        //public CustomerTypeCode CustomerTypeCode { get; set; }
        
        //[JsonPropertyName("customersizecode")]
        //public CustomerTypeCode CustomerTypeCode { get; set; }

        [JsonPropertyName("emailaddress1")]
        public object EmailAddress { get; set; }

        //[JsonPropertyName("owneremail")]
        //public string OwnerEmail { get; set; }

        //[JsonPropertyName("owneridname")]
        //public string OwnerIdName { get; set; }

        [JsonPropertyName("telephone1")]
        public string Phone { get; set; }

        private string DebuggerDisplay()
            => $"{AccountId} " +
               $"{Name} ";
    }
}
