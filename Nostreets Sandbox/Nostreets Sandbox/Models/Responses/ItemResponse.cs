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
    public class ItemResponse<T> : SuccessResponse
    {
        public ItemResponse() { }

        public ItemResponse(T item){
            Item = item;
        }

        public T Item { get; set; }

    }

    public class PairResponse<T, K> : ItemResponse<T>
    {
        
        public K Items2 { get; set; }


    }
}