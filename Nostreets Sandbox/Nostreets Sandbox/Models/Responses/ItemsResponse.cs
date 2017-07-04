using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Sandbox.Models.Responses
{
    /// <summary>
    /// This is an example of a Generic class that you will gain an understanding of
    /// as you progress through the training.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemsResponse<T> : SuccessResponse
    {
        public ItemsResponse()
        {

        }

        public ItemsResponse(List<T> items)
        {
            Items = items;
        }

        public List<T> Items { get; set; }
    }
}