using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.SparkPost
{


    public class Text
    {
        public Options Options { get; set; }
        public Content Content { get; set; }
        public Recipient[] Recipients { get; set; }
    }

    public class Options
    {
        public bool Sandbox { get; set; }
    }

    public class Content
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }

    public class Recipient
    {
        public string Address { get; set; }
    }

}
