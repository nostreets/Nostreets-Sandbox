using System.Collections.Generic;
using Nostreets_Services.Classes.Domain.Shopify;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IShopifyService
    {
        void CheckForUnregisteredCustomers();
        List<ShopifyCustomer> GetAllCustomers();
        void InsertUser(ShopifyCustomer customer);
        void RemoveCustomer(string customerId);
        void UpdateCustomer(ShopifyCustomer customer);
    }
}
