using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class Contact
    {
        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("contactid")]
        public Guid? ContactId { get; init; }

        //[JsonIgnore]
        //public Guid ParentAccountId
        //{
        //    get => ParentCustomerId?.Id ?? Guid.Empty;
        //    init =>
        //        ParentCustomerId = new ParentCustomerId
        //        {
        //            Id = value
        //        };
        //}

        // ReSharper disable once StringLiteralTypo
        //[JsonPropertyName("parentcustomerid")]
        //[JsonPropertyName("new_cpf")]
        //public ParentCustomerId ParentCustomerId { get; init; }
        [JsonIgnore]
        public Guid ParentAccountId { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("customertypecode")]
        public int CustomerTypeCode { get; init; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; init; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; init; }

        [JsonPropertyName("lastname")]
        public string LastName { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("mobilephone")]
        public object MobilePhone { get; init; }

        [JsonPropertyName("telephone1")]
        public string Phone { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("emailaddress1")]
        public string EmailAddress { get; init; }

        [JsonPropertyName("address1_city")]
        public string City { get; init; }

        [JsonPropertyName("address1_county")]
        public string County { get; init; }

        [JsonPropertyName("address1_line1")]
        public string AddressLine1 { get; init; }

        [JsonPropertyName("address1_line2")]
        public string AddressLine2 { get; init; }

        [JsonPropertyName("address1_line3")]
        public object AddressLine3 { get; init; }

        [JsonPropertyName("address1_postalcode")]
        public string Postcode { get; init; }

        private string DebuggerDisplay()
            => //$"{AccountId} " +
               $"{FirstName} {LastName}";
    }
}
