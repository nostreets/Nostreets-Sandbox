using NostreetsORM.Interfaces;
using NostreetsORM.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web;

namespace NostreetsORM
{
    public class DBService<T> : BaseService, IDBService<T>
    {
        public DBService() : base()
        {
            try
            {
                _path = Path.GetFullPath(Path.Combine(HttpContext.Current.Server.MapPath("~"), "../NostreetsORM/"));
                if (!CheckIfTableExist(typeof(T)))
                {
                    CreateTables(typeof(T));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DBService(string connectionKey) : base(connectionKey)
        {
            _path = Path.GetFullPath(Path.Combine(HttpContext.Current.Server.MapPath("~"), "../NostreetsORM/"));
            try
            {
                if (!CheckIfTableExist(typeof(T)))
                {
                    CreateTables(typeof(T));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DBService(string connectionKey, string directoryPath) : base(connectionKey)
        {
            try
            {
                _path = Path.GetFullPath(Path.Combine(directoryPath, "../NostreetsORM/"));
                if (!CheckIfTableExist(typeof(T)))
                {
                    CreateTables(typeof(T));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Type _type = typeof(T);
        private string _path = null;

        private string DeterminSQLType(Type type, string parentTable)
        {
            string statement = null;
            if ((type.BaseType.Name == "Enum" || type.IsClass) && (type.Name != "String" && type.Name != "Char"))
            {
                statement = "INT";
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
            }

            return statement;
        }

        private bool CheckIfTableExist(Type type)
        {
            string sqlTemp = String.Join(" ", File.ReadAllLines(_path + "/Queries/CheckIfTableExist.sql"));
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

        private void CreateProcdures(Type type)
        {

            string sqlInsertTemp = String.Join(" ", File.ReadAllLines(_path + "/Queries/InsertProcdure.sql"));
            string sqlUpdateTemp = String.Join(" ", File.ReadAllLines(_path + "/Queries/UpdateProcdure.sql"));
            string sqlSelectTemp = String.Join(" ", File.ReadAllLines(_path + "/Queries/SelectProcdure.sql"));
            string sqlDeleteTemp = String.Join(" ", File.ReadAllLines(_path + "/Queries/DeleteProcdure.sql"));
            string sqlUpdateNullCheckTemp = String.Join(" ", File.ReadAllLines(_path + "/Queries/NullCheckForUpdate.sql")) + " ";

            
            string[] temps = { sqlInsertTemp, sqlSelectTemp, sqlSelectTemp, sqlUpdateTemp, sqlDeleteTemp };

            for (int x = 0; x < temps.Length; x++)
            {

                string inputParams = null;
                string columns = null;
                string values = null;
                string select = null;
                string joins = null;
                string deletes = null;
                string updatesParams = null;

                List<string> inputs = new List<string>();
                List<string> colm = new List<string>();
                List<string> val = new List<string>();
                List<string> sel = new List<string>();
                List<string> jns = new List<string>();
                List<string> dels = new List<string>();
                List<string> updtParams = new List<string>();
                List<string> innerUpdt = new List<string>();

                string query = null;


                if (type.IsClass && (type.Name != "String" || type.Name != "Char"))
                {

                    var props = type.GetProperties();

                    for (int i = 0; i < props.Length; i++)
                    {
                        string PK = (props[i].PropertyType.BaseType.Name == "Enum" ? "Id" : props[i].Name);

                        if (i > 0)
                        {
                            inputs.Add("@" + props[i].Name + " " + DeterminSQLType(props[i].PropertyType, type.Name) + (i == props.Length - 1 ? "" : ","));

                            colm.Add(props[i].Name + ((props[i].PropertyType.BaseType.Name == "Enum" || props[i].PropertyType.IsClass) && (props[i].PropertyType.Name != "String" && props[i].PropertyType.Name != "Char") ? "Id" : "") + (i == props.Length - 1 ? "" : ","));

                            val.Add("@" + props[i].Name + (i == props.Length - 1 ? "" : ","));
                        }

                        updtParams.Add("@" + props[i].Name + DeterminSQLType(props[i].PropertyType, type.Name) + (i == 0 ? "" : " = NULL") + (i == props.Length - 1 ? "" : ","));

                        innerUpdt.Add("SET " + props[i].Name + ((props[i].PropertyType.BaseType.Name == "Enum" || props[i].PropertyType.IsClass) && (props[i].PropertyType.Name != "String" && props[i].PropertyType.Name != "Char") ? "Id" : "") + " = @" + props[i].Name + " WHERE " + type.Name + "s." + props[0].Name + " = @" + props[0].Name);


                        if ((props[i].PropertyType.BaseType.Name == "Enum" || props[i].PropertyType.IsClass) && (props[i].PropertyType.Name != "String" && props[i].PropertyType.Name != "Char"))
                        {
                            jns.Add("Inner Join " + props[i].PropertyType.Name + "s AS " + props[i].Name + "Id On " + props[i].Name + "Id." + (props[i].PropertyType.BaseType.Name == "Enum" ? "Id" : props[i].Name + "Id") + " = " + type.Name + "s." + props[i].Name + "Id");

                            if (!props[i].PropertyType.Namespace.Contains("System") && props[i].PropertyType.BaseType.Name != "Enum")
                            {
                                dels.Add("Delete " + props[i].Name + "s Where " + PK + " = (Select " + PK + " From " + type.Name + " Where " + PK + " = @" + PK + ")"); 
                            }
                        }

                        sel.Add(type.Name + "s.[" + props[i].Name + ((props[i].PropertyType.BaseType.Name == "Enum" || props[i].PropertyType.IsClass) && (props[i].PropertyType.Name != "String" && props[i].PropertyType.Name != "Char") ? "Id" : "") + "]" + (i == props.Length - 1 ? " " : ","));
                    }

                    inputParams = String.Join(" ", inputs.ToArray());
                    columns = String.Join(" ", colm.ToArray());
                    values = String.Join(" ", val.ToArray());
                    select = String.Join(" ", sel.ToArray());
                    joins = String.Join(" ", jns.ToArray());
                    deletes = String.Join(" ", dels.ToArray());
                    updatesParams = String.Join(" ", updtParams.ToArray());

                    if (temps[x] == sqlInsertTemp)
                    {
                        query = String.Format(temps[x], type.Name, inputParams, DeterminSQLType(props[0].PropertyType, type.Name), type.Name, columns, values);
                    }
                    else if (temps[x] == sqlSelectTemp)
                    {
                        if (x == 1)
                        {
                            query = String.Format(temps[x], type.Name, select, joins, "", "", "All");
                        }
                        else
                        {
                            query = String.Format(temps[x], type.Name, select, joins, "@" + props[0].Name + " " + DeterminSQLType(props[0].PropertyType, type.Name), "Where " + type.Name + "s." + props[0].Name + " = @" + props[0].Name, "ById");
                        }
                    }
                    else if (temps[x] == sqlUpdateTemp)
                    {

                        string innerQuery = null;
                        for (int q = 1; q < innerUpdt.Count; q++)
                        {
                            innerQuery += String.Format(sqlUpdateNullCheckTemp, type.Name, innerUpdt[q], props[q].Name);
                        }

                        query = String.Format(temps[x], type.Name, "@Id INT, " + inputParams, innerQuery);
                    }
                    else if (temps[x] == sqlDeleteTemp)
                    {

                        query = String.Format(temps[x], type.Name, props[0].Name, DeterminSQLType(props[0].PropertyType, type.Name), deletes);
                    }


                }
                else if (type.BaseType.Name == "Enum")
                {

                    inputs.Add("@Value " + DeterminSQLType(typeof(string), type.Name));
                    updtParams.Add("@Id " + DeterminSQLType(typeof(int), type.Name) + ", @Value " + DeterminSQLType(typeof(string), type.Name) + " = Null");
                    innerUpdt.Add("SET Value = @Value WHERE " + type.Name + "s.Id = @Id");
                    colm.Add("Value");
                    val.Add("@Value");
                    sel.Add(type.Name + "s.[Id], " + type.Name + "s.[Value]");


                    inputParams = String.Join(" ", inputs.ToArray());
                    columns = String.Join(" ", colm.ToArray());
                    values = String.Join(" ", val.ToArray());
                    select = String.Join(" ", sel.ToArray());
                    joins = String.Join(" ", jns.ToArray());
                    deletes = String.Join(" ", dels.ToArray());
                    updatesParams = String.Join(" ", updtParams.ToArray());

                    if (temps[x] == sqlInsertTemp)
                    {
                        query = String.Format(temps[x], type.Name, inputParams, DeterminSQLType(typeof(int), type.Name), type.Name, columns, values);
                    }
                    else if (temps[x] == sqlSelectTemp)
                    {
                        if (x == 1)
                        {
                            query = String.Format(temps[x], type.Name, select, "", "", "", "All");
                        }
                        else
                        {
                            query = String.Format(temps[x], type.Name, select, "", "@Id " + DeterminSQLType(typeof(int), type.Name), "Where " + type.Name + "s.Id = @Id", "ById");
                        }
                    }
                    else if (temps[x] == sqlUpdateTemp)
                    {

                        string innerQuery = null;
                        for (int num = 0; num < innerUpdt.Count; num++)
                        {

                            innerQuery += String.Format(sqlUpdateNullCheckTemp, type.Name, innerUpdt[num], "Value");
                        }

                        query = String.Format(temps[x], type.Name, " @Id INT, " + inputParams, innerQuery);
                    }
                    else if (temps[x] == sqlDeleteTemp)
                    {

                        query = String.Format(temps[x], type.Name, "Id", DeterminSQLType(typeof(int), type.Name), deletes);
                    }
                }


                DataProvider.ExecuteCmd(() => Connection,
                   query,
                    null,
                    (reader, set) =>
                    {
                        object id = DataMapper<object>.Instance.MapToObject(reader);
                    },
                    null, mod => mod.CommandType = System.Data.CommandType.Text);
            }

        }

        private Dictionary<string, string> CreateTables(Type type)
        {
            Dictionary<string, string> result = null;
            List<string> columns = new List<string>();
            List<string> FKs = new List<string>();

            if (CheckIfTableExist(type))
            {
                result = new Dictionary<string, string>();
                result.Add("Name", type.Name + "s");
                if (type.IsClass && (type.Name != "String" || type.Name != "Char"))
                {
                    result.Add("PK", type.GetProperties()[0].Name);
                }
                else if (type.BaseType.Name == "Enum")
                {
                    result.Add("PK", "Id");
                }
                return result;
            }
            else if (type.IsClass && (type.Name != "String" || type.Name != "Char"))
            {
                PropertyInfo[] props = type.GetProperties();
                string endingTable = "NOT NULL, CONSTRAINT [PK_" + type.Name + "s] PRIMARY KEY CLUSTERED ([" + props[0].Name + "] ASC)";


                foreach (var item in props)
                {
                    string FK = null;

                    if (item.PropertyType.IsClass && (item.PropertyType.Name != "String" && item.PropertyType.Name != "Char") || item.PropertyType.BaseType.Name == "Enum")
                    {
                        Dictionary<string, string> normalizedTbl = CreateTables(item.PropertyType);
                        FK = "CONSTRAINT [FK_" + type.Name + "s_" + item.Name + "] FOREIGN KEY ([" + item.Name + "Id]) REFERENCES [dbo].[" + normalizedTbl["Name"] + "] ([" + normalizedTbl["PK"] + "])";
                    }

                    columns.Add(String.Format(File.ReadAllText(_path + "/Queries/CreateColumn.sql"),
                        (item.PropertyType.BaseType.Name == "Enum" || item.PropertyType.IsClass) && (item.PropertyType.Name != "String" && item.PropertyType.Name != "Char") ? item.Name + "Id" : item.Name,
                        DeterminSQLType(item.PropertyType, type.Name),
                        props[0] == item ? "IDENTITY (1, 1) NOT NULL, " : props[props.Length - 1] == item ? endingTable + "," + String.Join(", ", FKs.ToArray()) : "NOT NULL, "));
                    if (FK != null)
                    {
                        FKs.Add(FK);
                    }
                }
            }
            else if (type.BaseType.Name == "Enum")
            {
                string endingTable = "NOT NULL, CONSTRAINT [PK_" + type.Name + "s] PRIMARY KEY CLUSTERED ([Id] ASC)";
                for (int i = 0; i < 2; i++)
                {
                    columns.Add(String.Format(File.ReadAllText(_path + "/Queries/CreateColumn.sql"), i == 0 ? "Id" : "Value", DeterminSQLType(i == 0 ? typeof(int) : typeof(string), type.Name),
                    i == 0 ? "IDENTITY (1, 1) NOT NULL, " : endingTable));
                }
            }


            string table = String.Concat(columns.ToArray());
            string query = String.Format(String.Join(" ", File.ReadAllLines(_path + "/Queries/CreateTable.sql")), type.Name, table);



            int isTrue = 0;
            DataProvider.ExecuteCmd(() => Connection,
               query,
                null,
                (reader, set) =>
                {
                    isTrue = reader.GetSafeInt32(0);
                },
                null,
                mod => mod.CommandType = System.Data.CommandType.Text);

            if (isTrue == 1)
            {
                CreateProcdures(type);
                result = new Dictionary<string, string>();
                result.Add("Name", type.Name + "s");
                if (type.IsClass && (type.Name != "String" || type.Name != "Char"))
                {
                    result.Add("PK", type.GetProperties()[0].Name);
                }
                else if (type.BaseType.Name == "Enum")
                {
                    var fields = type.GetFields();
                    result.Add("PK", "Id");
                    for (int i = 1; i < fields.Length; i++)
                    {
                        DataProvider.ExecuteNonQuery(() => Connection,
                                        "dbo." + type.Name + "s_Insert",
                                        (param) => param.Add(new SqlParameter("Value", fields[i].Name)),
                                        null);
                    }
                }
            }

            return result;
        }

        public List<T> GetAll()
        {
            List<T> list = null;
            DataProvider.ExecuteCmd(() => Connection, "dbo." + _type.Name + "s_SelectAll",
                null,
                (reader, set) =>
                {
                    T chart = DataMapper<T>.Instance.MapToObject(reader);
                    if (list == null) { list = new List<T>(); }
                    list.Add(chart);
                });
            return list;
        }

        public void Delete(object id)
        {
            DataProvider.ExecuteNonQuery(() => Connection, "dbo." + _type.Name + "s_Delete",
                param => param.Add(new SqlParameter(typeof(T).GetProperties()[0].Name, typeof(T).GetProperties()[0].GetValue(id))));
        }

        public T Get(object id)
        {
            T chart = default(T);
            DataProvider.ExecuteCmd(() => Connection, "dbo." + _type.Name + "s_SelectById",
                param => param.Add(new SqlParameter(typeof(T).GetProperties()[0].Name, typeof(T).GetProperties()[0].GetValue(id))),
                (reader, set) =>
                {
                    chart = DataMapper<T>.Instance.MapToObject(reader);
                });
            return chart;
        }

        public object Insert(T model)
        {
            object id = 0;
            DataProvider.ExecuteCmd(() => Connection, "dbo." + _type.Name + "s_Insert",
                       param => {
                           foreach (var prop in typeof(T).GetProperties()) {
                               if (prop.Name != "Id")
                               {
                                   param.Add(new SqlParameter(prop.Name, prop.GetValue(model))); 
                               }
                           }
                       },
                      (reader, set) => {
                          id = DataMapper<object>.Instance.MapToObject(reader);
                      });
            return id;
        }

        public void Update(object model)
        {
            DataProvider.ExecuteNonQuery(() => Connection, "dbo." + _type.Name + "s_Update",
                       param => {
                           param.Add(new SqlParameter(typeof(T).GetProperties()[0].Name, typeof(T).GetProperties()[0].GetValue(model)));
                           foreach(var prop in typeof(T).GetProperties()) {
                               param.Add(new SqlParameter(prop.Name, prop.GetValue(model)));
                           }
                       }
                       );
        }

    }
}