using Nostreets_Services.Interfaces.Sql;
using Nostreets_Services.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace Nostreets_Services.Services
{
    public abstract class BaseService
    {
        public BaseService()
        {
            _connectionKey = "DefaultConnection";
        }

        public BaseService(string connectionKey) { _connectionKey = connectionKey; }

        private string _connectionKey;
        private string _path = Path.GetFullPath(Path.Combine(HttpContext.Current.Server.MapPath("~"), "../Nostreets Services/Querys/"));

        public SqlConnection Connection { get { return new SqlConnection(WebConfigurationManager.ConnectionStrings[_connectionKey].ConnectionString); } }

        protected static IDao DataProvider
        {

            get { return Providers.DataProvider.SqlInstance; }
        }

        private string DeterminSQLType(Type type)
        {
            string statement = null;
            switch (type.Name)
            {
                case "String":
                    statement = "NVARCHAR (2000)";
                    break;

                case "Int16":
                    statement = "SMALLINT";
                    break;

                case "Int32":
                    statement = "INT";
                    break;

                case "Bool":
                    statement = "BIT";
                    break;

                case "DateTime":
                    statement = "DATETIME2 (7)  CONSTRAINT [DF_Charts_DateCreated] DEFAULT (getutcdate())";
                    break;

                default:
                    statement = "NVARCHAR (MAX)";
                    break;
            }

            return statement;
        }

        public bool CheckIfTableExist(Type type)
        {
            string sqlTemp = String.Join(" ", File.ReadAllLines(_path + "CheckIfTableExist.sql"));
            string query = String.Format(sqlTemp, type.Name);

            int isTrue = 0;
            DataProvider.ExecuteCmd(() => Connection,
               query,
                null,
                (reader, set) =>
                {
                    isTrue = reader.GetSafeInt32(0);
                },
                null, mod => mod.CommandType = System.Data.CommandType.Text);

            if (isTrue == 1) { return true; }

            return false;
        }

        public void CreateTables(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            List<string> columns = new List<string>();
            string endingTable = "NOT NULL, CONSTRAINT [PK_" + type.Name + "s] PRIMARY KEY CLUSTERED ([" + props[0].Name + "] ASC)";

            foreach (var item in props)
            {
                columns.Add(String.Format(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Querys/CreateColumn.sql")), item.Name, DeterminSQLType(item.PropertyType),
                    props[0] == item ? "IDENTITY (1, 1) NOT NULL, " : props[props.Length - 1] == item ? endingTable : "NOT NULL, "));
            }
            string table = String.Concat(columns.ToArray());
            string query = String.Format(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Querys/CreateTable.sql")), type.Name + "s", table);

            short isTrue;
            DataProvider.ExecuteCmd(() => Connection,
               query,
                param => param.AddRange(new[]
                {
                    new SqlParameter { SqlDbType = System.Data.SqlDbType.Bit, Direction = System.Data.ParameterDirection.Output, ParameterName = "IsTrue" }
                }),
                null,
                param => short.TryParse(param["@IsTrue"].Value.ToString(), out isTrue),
                mod => mod.CommandType = System.Data.CommandType.Text);
        }

    }
}