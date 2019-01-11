using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Classes.Domain.Plaid
{

    public class TransactionRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        public string Secret { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("start_date")]
        public string StartDate { get; set; }
        [JsonProperty("end_date")]
        public string EndDate { get; set; }
        public Options Options { get; set; }
    }

    public class Options
    {
        public int Count { get; set; }
        public int Offset { get; set; }
    }


    public class TransactionsResponse
    {
        public Account[] Accounts { get; set; }
        public Transaction[] Transactions { get; set; }
        public Item Item { get; set; }
        [JsonProperty("total_transactions")]
        public int TotalTransactions { get; set; }
        [JsonProperty("request_id")]
        public string RequestId { get; set; }
    }

    public class Account
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        public Balances Balances { get; set; }
        public string Mask { get; set; }
        public string Name { get; set; }
        [JsonProperty("official_name")]
        public string OfficialName { get; set; }
        public string Subtype { get; set; }
        public string Type { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        public float Amount { get; set; }
        public string[] Category { get; set; }
        [JsonProperty("category_id")]
        public string CategoryId { get; set; }
        public string Date { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        [JsonProperty("payment_meta")]
        public object PaymentMeta { get; set; }
        public bool Pending { get; set; }
        [JsonProperty("pending_transaction_id")]
        public string PendingTransactionId { get; set; }
        [JsonProperty("account_owner")]
        public string AccountOwner { get; set; }
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }
        [JsonProperty("transaction_type")]
        public string TransactionType { get; set; }
    }

    public class Item
    {
        [JsonProperty("available_products")]
        public string[] AvailableProducts { get; set; }
        [JsonProperty("billed_products")]
        public string[] BilledProducts { get; set; }
        public object Error { get; set; }
        [JsonProperty("institution_id")]
        public string InstitutionId { get; set; }
        [JsonProperty("item_id")]
        public string ItemId { get; set; }
        public string Webhook { get; set; }
    }

    public class Balances
    {
        public int Available { get; set; }
        public int Current { get; set; }
        public object Limit { get; set; }
    }

    public class Location
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
    }

    public class PaymentMeta
    { }
    
}
