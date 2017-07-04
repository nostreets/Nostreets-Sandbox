using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Sandbox.Models.ViewModels
{
    public class MetaTagViewModel : BaseViewModel
    {
        public int OwnerId { get; set; }
        public int OwnerTypeId { get; set; }
    }
}