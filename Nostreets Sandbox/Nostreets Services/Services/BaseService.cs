using Nostreets_Services.Domain.Cards;
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

        public BaseService(string connectionKey) {
          
            _connectionKey = connectionKey;

            //if (!CheckIfTableExist(typeof(StyledCard)))
            //{
            //    var table = CreateTables(typeof(StyledCard));
            //}
        }

        private string _connectionKey;
        private string _path = Path.GetFullPath(Path.Combine(HttpContext.Current.Server.MapPath("~"), "../Nostreets Services/Querys/"));

        public SqlConnection Connection { get { return new SqlConnection(WebConfigurationManager.ConnectionStrings[_connectionKey].ConnectionString); } }

        protected static IDao DataProvider
        {

            get { return Providers.DataProvider.SqlInstance; }
        }

        private string DeterminSQLType(Type type, string parentTable, out string FKs)
        {
            string statement = null;
            if (type.IsByRef && (type.Name != "String" || type.Name != "Char"))
            {
                Dictionary<string,string> table = CreateTables(type);
                statement = "INT";
                FKs = "CONSTRAINT [FK_" + parentTable + "s_" + table["Name"] + "] FOREIGN KEY ([" + type.Name + "]) REFERENCES [dbo].[" + table["Name"] + "] ([" + table["PK"] + "])";
            }
            else
            {
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
                        statement = "DATETIME2 (7)  CONSTRAINT [DF_" + parentTable + "s_" + type.Name + "] DEFAULT (getutcdate())";
                        break;

                    default:
                        statement = "NVARCHAR (MAX)";
                        break;
                }
                FKs = null;
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

        public Dictionary<string, string> CreateTables(Type type)
        {
            Dictionary<string, string> result = null;
            PropertyInfo[] props = type.GetProperties();
            List<string> columns = new List<string>();
            List<string> FKs = new List<string>();

            string endingTable = "NOT NULL, CONSTRAINT [PK_" + type.Name + "s] PRIMARY KEY CLUSTERED ([" + props[0].Name + "] ASC)";

            foreach (var item in props)
            {
                string FK = null;
                columns.Add(String.Format(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Querys/CreateColumn.sql")), item.Name, DeterminSQLType(item.PropertyType, type.Name, out FK),
                    props[0] == item ? "IDENTITY (1, 1) NOT NULL, " : props[props.Length - 1] == item ? String.Concat(endingTable, FKs.ToArray()) : "NOT NULL, "));
                if (FK != null)
                {
                    FKs.Add(FK);
                }
            }
            string table = String.Concat(columns.ToArray());
            string query = String.Format(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Querys/CreateTable.sql")), type.Name + "s", table);

            short isTrue = 0;
            DataProvider.ExecuteCmd(() => Connection,
               query,
                param => param.AddRange(new[]
                {
                    new SqlParameter { SqlDbType = System.Data.SqlDbType.Bit, Direction = System.Data.ParameterDirection.Output, ParameterName = "IsTrue" }
                }),
                null,
                param => short.TryParse(param["@IsTrue"].Value.ToString(), out isTrue),
                mod => mod.CommandType = System.Data.CommandType.Text);

            if (isTrue == 1) {
                result = new Dictionary<string, string>();
                result.Add("Name", type.Name + "s");
                result.Add("PK", type.GetProperties()[0].Name + "s");
            }

            return result;
        }

    }
}