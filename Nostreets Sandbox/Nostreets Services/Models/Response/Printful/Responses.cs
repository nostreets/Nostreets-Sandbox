using Newtonsoft.Json;
using System.Collections.Generic;

namespace NostreetsServices.Models.Request.Printful
{
    public class Costs
    {
        [JsonProperty("additionalfee")]
        public string Additionalfee { get; set; }
        public string Currency { get; set; }
        public string Digitization { get; set; }
        public string Discount { get; set; }
        [JsonProperty("fulfillment_fee")]
        public string FulfillmentFee { get; set; }
        public string Shipping { get; set; }
        public string Subtotal { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }
        public string Vat { get; set; }
    }

    public class PrintfulOrderResponse
    {
        public int Code { get; set; }
        public IList<object> Extra { get; set; }
        public Paging Paging { get; set; }
        public IList<Result> Result { get; set; }
    }

    public class File
    {
        public int Created { get; set; }
        public int Dpi { get; set; }
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        public string Hash { get; set; }
        public int Height { get; set; }
        public int Id { get; set; }
        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }
        public int Size { get; set; }
        public string Status { get; set; }
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        public string Type { get; set; }
        public object Url { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }
    }

    public class PrintfulItem
    {
        public bool Discontinued { get; set; }
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }
        [JsonProperty("external_variant_id")]
        public string ExternalVariantId { get; set; }
        public IList<File> Files { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Option> Options { get; set; }
        [JsonProperty("out_of_stock")]
        public bool OutOfStock { get; set; }
        [JsonProperty("out_of_stock_eu")]
        public bool OutOfStockEu { get; set; }
        public string Price { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        [JsonProperty("retail_price")]
        public string RetailPrice { get; set; }
        public object Sku { get; set; }
        [JsonProperty("sync_variant_id")]
        public int SyncVariantId { get; set; }
        [JsonProperty("variant_id")]
        public int Variantid { get; set; }
    }

    public class ShipItem
    {
        [JsonProperty("item_id")]
        public int ItemId { get; set; }
        public int Picked { get; set; }
        public int Printed { get; set; }
        public int Puantity { get; set; }
    }

    public class Option
    {
        public string Id { get; set; }
        public object Value { get; set; }
    }

    public class Paging
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class Product
    {
        public string Image { get; set; }
        public string Name { get; set; }
        [JsonProperty("product_id")]
        public int ProductId { get; set; }
        [JsonProperty("variant_id")]
        public int VariantId { get; set; }
    }

    public class Recipient
    {
        public string Address1 { get; set; }
        public object Address2 { get; set; }
        public string City { get; set; }
        public object Company { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("country_name")]
        public string NountryName { get; set; }
        public object Email { get; set; }
        public string Name { get; set; }
        public object Phone { get; set; }
        [JsonProperty("state_code")]
        public string StateCode { get; set; }
        [JsonProperty("state_name")]
        public string StateName { get; set; }
        public string Zip { get; set; }
    }
    public class Result
    {
        [JsonProperty("can_change_hold")]
        public bool CanChangeHold { get; set; }
        public Costs Costs { get; set; }
        public int Created { get; set; }
        [JsonProperty("dashboard_url")]
        public string DashboardUrl { get; set; }
        public object Error { get; set; }
        [JsonProperty("eu_route_required")]
        public bool EuRouteRequired { get; set; }
        [JsonProperty("eu_routed")]
        public bool EuRouted { get; set; }
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }
        public object Gift { get; set; }
        [JsonProperty("has_discontinued_items")]
        public bool HasDiscontinuediIems { get; set; }
        public int Id { get; set; }
        [JsonProperty("is_sample")]
        public bool IsSample { get; set; }
        public IList<PrintfulItem> Items { get; set; }
        [JsonProperty("needs_approval")]
        public bool NeedsApproval { get; set; }
        [JsonProperty("needs_approval_eu")]
        public bool NeedsApprovalEu { get; set; }
        [JsonProperty("not_synced")]
        public bool NotSynced { get; set; }
        public object Notes { get; set; }
        [JsonProperty("packing_slip")]
        public object PackingSlip { get; set; }
        public Recipient Recipient { get; set; }
        [JsonProperty("retail_costs")]
        public RetailCosts RetailCosts { get; set; }
        public IList<Shipment> Shipments { get; set; }
        public string Shipping { get; set; }
        public string status { get; set; }
        public int Store { get; set; }
        public int Updated { get; set; }
    }

    public class RetailCosts
    {
        public string Currency { get; set; }
        public string Discount { get; set; }
        public string Shipping { get; set; }
        public string Subtotal { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }
        public string Vat { get; set; }
    }
    public class Shipment
    {
        public string Carrier { get; set; }
        public int Created { get; set; }
        public int Id { get; set; }
        public IList<ShipItem> Items { get; set; }
        public string Location { get; set; }
        [JsonProperty("packing_slip_url")]
        public string PackingSlipUrl { get; set; }
        public bool Reshipment { get; set; }
        public string Service { get; set; }
        [JsonProperty("ship_date")]
        public string ShipDate { get; set; }
        [JsonProperty("shipped_at")]
        public int ShippedAt { get; set; }
        public string Status { get; set; }
        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; }
        [JsonProperty("tracking_url")]
        public string TrackingUrl { get; set; }
    }
}