using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nostreets_Services.Domain.Shopify
{
    public class Address
    {
        public int Id { get; set; }
        [JsonProperty("customer_id")]
        public int CustomerId { get; set; }
        [JsonProperty("first_name")]
        public object FirstName { get; set; }
        [JsonProperty("last_name")]
        public object LastName { get; set; }
        public object Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("country_name")]
        public string CountryName { get; set; }
        public bool Default { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [JsonProperty("accepts_marketing")]
        public bool AcceptsMarketing { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("orders_count")]
        public int OrdersCount { get; set; }
        public string State { get; set; }
        [JsonProperty("total_spent")]
        public string TotalSpent { get; set; }
        [JsonProperty("last_order_id")]
        public int LastOrderId { get; set; }
        public object Note { get; set; }
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonProperty("multipass_identifier")]
        public object MultipassIdentifier { get; set; }
        [JsonProperty("tax_exempt")]
        public bool TaxExempt { get; set; }
        public object Phone { get; set; }
        public string Tags { get; set; }
        [JsonProperty("last_order_name")]
        public string LastOrderName { get; set; }
        public string Currency { get; set; }
        public IList<Address> Addresses { get; set; }
        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphqlApiId { get; set; }
        [JsonProperty("default_address")]
        public Address DefaultAddress { get; set; }
    }
}