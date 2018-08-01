using NostreetsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public class Error
    {
        private Error() { }

        public Error(Exception ex)
        {
           List<Error> allErrors = Static.ParseStackTrace<Error>(ex.StackTrace, (fr, ty, me, pl, ps, fl, ln) =>
                {
                    Error err = new Error
                    {

                        Frame = fr,
                        Type = ty,
                        Method = me,
                        ParameterList = pl,
                        Parameters = ps,
                        File = fl,
                        Line = ln
                    };

                });


        }


        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Time { get; set; }
        public string Frame { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string ParameterList { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; set; }
        public string File { get; set; }
        public string Line { get; set; }
        public IEnumerable<Error> InnerErrors { get; set; }

    }
}
