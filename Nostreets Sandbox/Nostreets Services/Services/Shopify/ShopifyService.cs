using System;
using System.Collections.Generic;
using System.Linq;
using Nostreets_Services.Domain.Shopify;
using Nostreets_Services.Domain.Users;
using Nostreets_Services.Domain.Web;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Interfaces;
using RestSharp;

namespace Nostreets_Services.Services.Shopify
{
    public class ShopifyService : IShopifyService
    {
        public ShopifyService(string domain, string apiKey, IUserService userSrv)
        {
            _userSrv = userSrv;
            Domain = domain;
            Headers.Add("authentication", apiKey);
            //Headers.Add("contentType", "JSON");
        }

        public ShopifyService(string domain, string apiKey, IUserService userSrv, IDBService<RequestError> errorLog)
        {
            _userSrv = userSrv;
            _errorLog = errorLog;
            Domain = domain;
            Headers.Add("authentication", apiKey);
            //Headers.Add("contentType", "JSON");
        }

        private string Domain { get; set; }

        private Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        private IDBService<RequestError> _errorLog = null;
        private IUserService _userSrv = null;
        private IRestResponse<T> Endpoint<T>(string endpoint, string method = null, object data = null) where T : new()
        {
            string url = Domain + endpoint;
            return url.RestSharpEndpoint<T>(method, data, "application/json", Headers);
        }

        public List<ShopifyCustomer> GetAllCustomers()
        {
            try
            {
                IRestResponse<List<ShopifyCustomer>> response = Endpoint<List<ShopifyCustomer>>("/admin/customers.json", "GET");
                if (response.IsSuccessful)
                    return response.Data;
                else
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                if (_errorLog != null)
                    _errorLog.Insert(new RequestError(null, Domain + "/admin/customers.json", ex));

                throw ex;
            }
        }

        public void InsertUser(ShopifyCustomer customer)
        {
            User user = new User()
            {
                LastLogIn = null,
                UserOrigin = UserOriginType.Shopify,
                Password = customer.Id.ToString(),
                UserName = customer.Email,
                Contact = new Contact()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PrimaryEmail = customer.Email,
                    PrimaryPhone = customer.Phone.ToString(),
                }
            };

            _userSrv.InsertUser(user);
        }

        public void RemoveCustomer(string customerId)
        {
            try
            {
                IRestResponse<List<ShopifyCustomer>> response = Endpoint<List<ShopifyCustomer>>("/admin/customers/#" + customerId + ".json", "DELETE");

                if (!response.IsSuccessful)
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                if (_errorLog != null)
                    _errorLog.Insert(new RequestError(null, Domain + "/admin/customers.json", ex));

                throw ex;
            }
        }

        public void UpdateCustomer(ShopifyCustomer customerId)
        {
            try
            {
                IRestResponse<List<ShopifyCustomer>> response = Endpoint<List<ShopifyCustomer>>("/admin/customers/#" + customerId + ".json", "PUT");

                if (!response.IsSuccessful)
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                if (_errorLog != null)
                    _errorLog.Insert(new RequestError(null, Domain + "/admin/customers.json", ex));

                throw ex;
            }
        }

        public void CheckForUnregisteredCustomers()
        {
            List<ShopifyCustomer> shopifyCustomers = GetAllCustomers();
            Dictionary<User, UserData> dbUsers = _userSrv.GetAllUsersWithUserData();
            List<ShopifyCustomer> customersNotInDB = shopifyCustomers.Where(a => !customerIsInDB(a)).ToList();

            foreach (ShopifyCustomer _customer in customersNotInDB)
            {
                _userSrv.RegisterAsync(new User()
                {
                    UserName = _customer.Email,
                    Password = _customer.Id.ToString(),
                    UserOrigin = UserOriginType.Shopify,
                    Contact = new Contact()
                    {
                        FirstName = _customer.FirstName,
                        LastName = _customer.LastName,
                        PrimaryEmail = _customer.Email,
                        PrimaryPhone = _customer.Phone.ToString(),
                        Address = new StreetAddress() {
                            OwnersFirstName = _customer.DefaultAddress.FirstName.ToString(),
                            OwnersLastName = _customer.DefaultAddress.LastName.ToString(),
                            AddressLine1 = _customer.DefaultAddress.Address1,
                            AddressLine2 = _customer.DefaultAddress.Address2,
                            City = _customer.DefaultAddress.City,
                            State = _customer.State,
                            ZipCode = int.Parse(_customer.DefaultAddress.Zip),
                            PhoneNumber = _customer.DefaultAddress.Phone
                        }
                    }
                });
            }

            bool customerIsInDB(ShopifyCustomer customer)
            {
                return dbUsers.Any(a => a.Key.Contact.PrimaryEmail == customer.Email || a.Value.ApiIDs["ShopifyId"] == customer.Id.ToString());
            }
        }
    }
}
