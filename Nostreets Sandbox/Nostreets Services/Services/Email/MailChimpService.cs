using Newtonsoft.Json;

using Nostreets_Services.Classes.Domain.Mailchimp;
using Nostreets_Services.Classes.Domain.Mailchimp.Lists;
using Nostreets_Services.Classes.Domain.Mailchimp.Members;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models;
using Nostreets_Services.Models.Requests;

using NostreetsExtensions.Helpers.Data;
using NostreetsExtensions.Utilities;

using RestSharp;
using RestSharp.Authenticators;

using System;
using System.Collections.Generic;
using System.Net;

namespace Nostreets_Services.Services.Email
{
    public class MailChimpService : SqlService, IMailChimpService
    {
        public MailChimpService(string domain, string apiKey)
        {
            Domain = domain;
            Headers.Add("authentication", apiKey);
            Headers.Add("contentType", "JSON");
        }

        private string Domain { get; set; }
        private Dictionary<string, string> Headers { get; set; }

        private IRestResponse<T> GenericRestSharp<T>(string url, string method = null, object data = null) where T : new()
        {
            #region Client

            RestClient rest = null;
            if (url != null)
            {
                rest = new RestClient(url);
            }
            else { throw new Exception("URL is not defined!"); }

            #endregion Client

            #region Request

            RestRequest request = new RestRequest();
            switch (method)
            {
                case "GET":
                    request.Method = Method.GET;
                    break;

                case "POST":
                    request.Method = Method.POST;
                    break;

                case "PATCH":
                    request.Method = Method.PATCH;
                    break;

                case "PUT":
                    request.Method = Method.PUT;
                    break;

                case "DELETE":
                    request.Method = Method.DELETE;
                    break;

                default:
                    request.Method = Method.GET;
                    break;
            };
            request.JsonSerializer = CustomSerializer.CamelCaseIngoreDictionaryKeys;
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            if (Headers != null)
            {
                foreach (var item in Headers)
                {
                    if (item.Key == "authentication")
                    {
                        rest.Authenticator = new HttpBasicAuthenticator("username", item.Key);
                    }
                    else if (item.Key == "contentType")
                    {
                        request.AddParameter(new Parameter { ContentType = item.Value });
                    }
                    else
                    {
                        request.AddParameter(new Parameter { Name = item.Key, Value = item.Value });
                    }
                }
            }

            #endregion Request

            return rest.Execute<T>(request);
        }

        public Member AddMember(MailChimpAddMemberRequest data, string listId)
        {
            #region Rest Call and Response

            if (data.Merge_Fields == null) { data.Merge_Fields = new Dictionary<string, string>(); }

            IRestResponse<Member> response = GenericRestSharp<Member>("lists/" + listId + "/members", "POST", data);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                return response.Data;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public void AddMember(MailChimpAddMemberRequest data, string listId, Func<Member, object> onSuccess, Func<IRestResponse, object> onError)
        {
            #region Rest Call and Response

            if (data.Merge_Fields == null) { data.Merge_Fields = new Dictionary<string, string>(); }

            IRestResponse<Member> response = GenericRestSharp<Member>("lists/" + listId + "/members", "POST", data);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public void AddMember(MailChimpAddMemberRequest data, string listId, Action<Member> onSuccess, Action<IRestResponse> onError)
        {
            #region Rest Call and Response

            if (data.Merge_Fields == null) { data.Merge_Fields = new Dictionary<string, string>(); }

            IRestResponse<Member> response = GenericRestSharp<Member>("lists/" + listId + "/members", "POST", data);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public MailChimpList CreateList(MailChimpAddListRequest data)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>("lists", "POST", data);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                return response.Data;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public void CreateList(MailChimpAddListRequest data, Func<MailChimpList, object> onSuccess, Func<IRestResponse, object> onError)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>("lists", "POST", data);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public void CreateList(MailChimpAddListRequest data, Action<MailChimpList> onSuccess, Action<IRestResponse> onError)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>("lists", "POST", data);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public bool DeleteList(string listId)
        {
            #region Rest Call and Response

            IRestResponse<object> response = GenericRestSharp<object>("lists/" + listId, "DELETE");
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public bool DeleteMember(string listId, string memberHash)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>(Domain + "lists/" + listId + "/members/" + memberHash, "DELETE");
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;

            #endregion Rest Call and Response
        }

        public void GetAllMembers(string listId, Func<MemberCollection, object> onSuccess, Func<IRestResponse, object> onError)
        {
            IRestResponse<MemberCollection> response = GenericRestSharp<MemberCollection>(Domain + "lists/" + listId + "/members/");
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MemberCollection>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }
        }

        public void GetAllMembers(string listId, Action<MemberCollection> onSuccess, Action<IRestResponse> onError)
        {
            IRestResponse<MemberCollection> response = GenericRestSharp<MemberCollection>(Domain + "lists/" + listId + "/members/");
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MemberCollection>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }
        }

        public MemberCollection GetAllMembers(string listId)
        {
            IRestResponse<MemberCollection> response = GenericRestSharp<MemberCollection>(Domain + "lists/" + listId + "/members/");
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MemberCollection>(response.Content); }
                return response.Data;
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        public PagedList<Member> GetAllMembers(string listId, int pgIndex = 0, int pgSize = 20)
        {
            string endpoint = Domain + "lists/" + listId + "/members/";
            if (pgSize != 0 || pgIndex != 0) { endpoint += "?"; }
            if (pgSize != 0) { endpoint += "count=" + pgSize + "&"; }
            if (pgIndex != 0) { endpoint += "offset=" + (pgSize * pgIndex); }

            IRestResponse<MemberCollection> response = GenericRestSharp<MemberCollection>(endpoint);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                MemberCollection collection = response.Data;
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MemberCollection>(response.Content); }
                PagedList<Member> pagedList = new PagedList<Member>(collection.Members, pgIndex, pgSize);
                return pagedList;
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        public void GetListById(string listId, Func<MailChimpList, object> onSuccess, Func<IRestResponse, object> onError)
        {
            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>(Domain + "lists/" + listId);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                MailChimpList list = response.Data;
                onSuccess(list);
            }
            else
            {
                onError(response);
            }
        }

        public void GetListById(string listId, Action<MailChimpList> onSuccess, Action<IRestResponse> onError)
        {
            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>(Domain + "lists/" + listId);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                MailChimpList list = response.Data;
                onSuccess(list);
            }
            else
            {
                onError(response);
            }
        }

        public MailChimpList GetListById(string listId)
        {
            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>(Domain + "lists/" + listId);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                MailChimpList list = response.Data;
                return list;
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        public PagedList<MailChimpList> GetLists(int pgSize, int pgIndex)
        {
            #region Rest Call and Response

            string endpoint = Domain + "lists";
            if (pgSize != 0 || pgIndex != 0) { endpoint += "?"; }
            if (pgSize != 0) { endpoint += "count=" + pgSize; }
            if (pgIndex != 0) { endpoint += "offset=" + pgSize * pgIndex; }

            IRestResponse<ListCollection> response = GenericRestSharp<ListCollection>(endpoint);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<ListCollection>(response.Content); }
                ListCollection list = response.Data;
                PagedList<MailChimpList> pagedList = new PagedList<MailChimpList>(list.Lists, pgIndex, pgSize);
                return pagedList;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public ListCollection GetLists()
        {
            #region Rest Call and Response

            string endpoint = Domain + "lists?count=" + int.MaxValue;

            IRestResponse<ListCollection> response = GenericRestSharp<ListCollection>(endpoint);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<ListCollection>(response.Content); }
                ListCollection list = response.Data;
                return list;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public void GetLists(int pgSize, int pgIndex, Func<ListCollection, object> onSuccess, Func<IRestResponse, object> onError)
        {
            #region Rest Call and Response

            string endpoint = Domain + "lists?count=" + int.MaxValue;

            IRestResponse<ListCollection> response = GenericRestSharp<ListCollection>(endpoint);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<ListCollection>(response.Content); }
                ListCollection list = response.Data;
                onSuccess(list);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public void GetLists(int pgSize, int pgIndex, Action<ListCollection> onSuccess, Action<IRestResponse> onError)
        {
            #region Rest Call and Response

            string endpoint = Domain + "lists?count=" + int.MaxValue;

            IRestResponse<ListCollection> response = GenericRestSharp<ListCollection>(endpoint);

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<ListCollection>(response.Content); }
                ListCollection list = response.Data;
                onSuccess(list);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public Member GetMember(string listId, string memberId)
        {
            #region Rest Call and Response

            Member member = null;

            IRestResponse<Member> response = GenericRestSharp<Member>(Domain + "lists/" + listId + "/members/" + memberId);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                member = response.Data;
            }
            else
            {
                throw new Exception(response.Content);
            }
            return member;

            #endregion Rest Call and Response
        }

        public void GetMember(string listId, string memberId, Func<Member, object> onSuccess, Func<IRestResponse, object> onError)
        {
            #region Rest Call and Response

            IRestResponse<Member> response = GenericRestSharp<Member>(Domain + "lists/" + listId + "/members/" + memberId);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public void GetMember(string listId, string memberId, Action<Member> onSuccess, Action<IRestResponse> onError)
        {
            #region Rest Call and Response

            IRestResponse<Member> response = GenericRestSharp<Member>(Domain + "lists/" + listId + "/members/" + memberId);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public MailChimpList UpdateList(MailChimpAddListRequest data, string listId)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>("lists/" + listId, "PATCH", data);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                MailChimpList list = response.Data;
                return list;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public void UpdateList(MailChimpAddListRequest data, string listId, Func<MailChimpList, object> onSuccess, Func<IRestResponse, object> onError)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>("lists/" + listId, "PATCH", data);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                MailChimpList list = response.Data;
                onSuccess(list);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public void UpdateList(MailChimpAddListRequest data, string listId, Action<MailChimpList> onSuccess, Action<IRestResponse> onError)
        {
            #region Rest Call and Response

            IRestResponse<MailChimpList> response = GenericRestSharp<MailChimpList>("lists/" + listId, "PATCH", data);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<MailChimpList>(response.Content); }
                MailChimpList list = response.Data;
                onSuccess(list);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public Member UpdateMember(MailChimpAddMemberRequest data, string listId, string hashId)
        {
            #region Rest Call and Response

            IRestResponse<Member> response = GenericRestSharp<Member>("lists/" + listId + "/members/" + hashId, "PUT", data);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                Member list = response.Data;
                return list;
            }
            else
            {
                throw new Exception(response.Content);
            }

            #endregion Rest Call and Response
        }

        public void UpdateMember(MailChimpAddMemberRequest data, string listId, string hashId, Func<Member, object> onSuccess, Func<IRestResponse, object> onError)
        {
            #region Rest Call and Response

            IRestResponse<Member> response = GenericRestSharp<Member>("lists/" + listId + "/members/" + hashId, "PUT", data);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }

        public void UpdateMember(MailChimpAddMemberRequest data, string listId, string hashId, Action<Member> onSuccess, Action<IRestResponse> onError)
        {
            #region Rest Call and Response

            IRestResponse<Member> response = GenericRestSharp<Member>("lists/" + listId + "/members/" + hashId, "PUT", data);
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.NoContent)
            {
                if (response.Data == null) { response.Data = JsonConvert.DeserializeObject<Member>(response.Content); }
                onSuccess(response.Data);
            }
            else
            {
                onError(response);
            }

            #endregion Rest Call and Response
        }
    }
}