using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Enums
{
    public enum CardSize : int
    {
        Small = 1,
        Medium,
        Large
    }

    public enum ContentType : int
    {
        Text = 1,
        Image,
        Video
    }

    public enum AlignmentType : int
    {
        Left = 1,
        Middle,
        Right
    }
}
