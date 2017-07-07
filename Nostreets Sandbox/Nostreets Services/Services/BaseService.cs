using Nostreets_Services.Interfaces.Sql;
using Nostreets_Services.Providers;
using System;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Nostreets_Services.Services
{
    public abstract class BaseService
    {
        public BaseService() { _connectionKey = "DefaultConnection"; }

        public BaseService(string connectionKey) { _connectionKey = connectionKey; }

        private string _connectionKey;

        public SqlConnection Connection { get { return new SqlConnection(WebConfigurationManager.ConnectionStrings[_connectionKey].ConnectionString); } }

        protected static IDao DataProvider
        {

            get { return Providers.DataProvider.SqlInstance; }
        }

        void CreateClassTables(params Type[] type)
        {
            short isTrue;
            DataProvider.ExecuteCmd(() => Connection, 
                "",
                param => param.AddRange(new[]
                {
                    new SqlParameter()
                }),
                null,
                param => short.TryParse(param["@IsTrue"].Value.ToString(), out isTrue),
                mod => mod.CommandType = System.Data.CommandType.Text);
        }

    }
}