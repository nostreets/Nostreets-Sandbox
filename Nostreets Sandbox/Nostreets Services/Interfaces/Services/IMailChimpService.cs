using Nostreets_Services.Domain.Mailchimp;
using Nostreets_Services.Domain.Mailchimp.Lists;
using Nostreets_Services.Domain.Mailchimp.Members;
using Nostreets_Services.Models.Requests;
using Nostreets_Services.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IMailChimpService
    {
        void GetAllMembers(string listId, Func<MemberCollection, object> onSuccess, Func<IRestResponse, object> onError);

        void GetAllMembers(string listId, Action<MemberCollection> onSuccess, Action<IRestResponse> onError);

        MemberCollection GetAllMembers(string listId);

        PagedList<Member> GetAllMembers(string listId, int pgIndex = 0, int pgSize = 20);

        void GetListById(string listId, Func<MailChimpList, object> onSuccess, Func<IRestResponse, object> onError);

        void GetListById(string listId, Action<MailChimpList> onSuccess, Action<IRestResponse> onError);

        MailChimpList GetListById(string listId);

        bool DeleteMember(string listId, string memberHash);

        Member GetMember(string listId, string memberId);

        void GetMember(string listId, string memberId, Func<Member, object> onSuccess, Func<IRestResponse, object> onError);

        void GetMember(string listId, string memberId, Action<Member> onSuccess, Action<IRestResponse> onError);

        PagedList<MailChimpList> GetLists(int pgSize, int pgIndex);

        ListCollection GetLists();

        void GetLists(int pgSize, int pgIndex, Func<ListCollection, object> onSuccess, Func<IRestResponse, object> onError);

        void GetLists(int pgSize, int pgIndex, Action<ListCollection> onSuccess, Action<IRestResponse> onError);

        MailChimpList CreateList(MailChimpAddListRequest data);

        void CreateList(MailChimpAddListRequest data, Func<MailChimpList, object> onSuccess, Func<IRestResponse, object> onError);

        void CreateList(MailChimpAddListRequest data, Action<MailChimpList> onSuccess, Action<IRestResponse> onError);

        Member AddMember(MailChimpAddMemberRequest data, string listId);

        void AddMember(MailChimpAddMemberRequest data, string listId, Func<Member, object> onSuccess, Func<IRestResponse, object> onError);

        void AddMember(MailChimpAddMemberRequest data, string listId, Action<Member> onSuccess, Action<IRestResponse> onError);

        MailChimpList UpdateList(MailChimpAddListRequest data, string listId);

        void UpdateList(MailChimpAddListRequest data, string listId, Func<MailChimpList, object> onSuccess, Func<IRestResponse, object> onError);

        void UpdateList(MailChimpAddListRequest data, string listId, Action<MailChimpList> onSuccess, Action<IRestResponse> onError);

        bool DeleteList(string listId);
    }
}
