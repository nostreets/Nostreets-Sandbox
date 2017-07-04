using Nostreets_Services.Interfaces.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Providers
{
    public sealed class DataProvider
    {
        private DataProvider() { }

        public static IDao SqlInstance
        {
            get
            {
                return SqlDao.Instance;
            }
        }

    }
}
