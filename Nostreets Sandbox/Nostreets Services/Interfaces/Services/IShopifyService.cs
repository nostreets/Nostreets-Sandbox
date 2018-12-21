using System.Collections.Generic;
using Nostreets_Services.Domain.Shopify;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IShopifyService
    {
        List<ShopifyCustomer> GetCustomers();
        void InsertUser(ShopifyCustomer customer);
        void RemoveCustomer(string customerId);
        void UpdateCustomer(ShopifyCustomer customer);
    }
}
