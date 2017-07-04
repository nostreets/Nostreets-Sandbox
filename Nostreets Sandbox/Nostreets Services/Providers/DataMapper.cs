using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Providers
{
    public class DataMapper<T> where T : class
    {
        private System.Reflection.PropertyInfo[] props;

        private static readonly DataMapper<T> _instance = new DataMapper<T>();

        private DataMapper()
        {
            props = typeof(T).GetProperties(); //THIS WILL PRELOAD YOUR CLASS PROPERTIES
        }

        static DataMapper() { }

        public static DataMapper<T> Instance { get { return _instance; } }

        public T MapToObject(IDataReader reader)
        {
            IEnumerable<string> colname = reader.GetSchemaTable().Rows.Cast<DataRow>().Select(c => c["ColumnName"].ToString().ToLower()).ToList();
            var obj = Activator.CreateInstance<T>();
            foreach (var prop in props)
            {
                if (colname.Contains(prop.Name.ToLower()))
                {
                    Type typ = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    
                    if (typ.Name != "String" && typ.IsClass)
                    {
                        dynamic pin = reader.GetString(reader.GetOrdinal(prop.Name));
                        prop.SetValue(obj, JsonConvert.DeserializeObject(reader.GetString(reader.GetOrdinal(prop.Name)) ?? "", typ), null);
                    }
                    else if (reader[prop.Name] != DBNull.Value)
                    {
                        if (reader[prop.Name].GetType() == typeof(decimal)) { prop.SetValue(obj, (reader.GetDouble(prop.Name)), null); }
                        else { prop.SetValue(obj, (reader.GetValue(reader.GetOrdinal(prop.Name)) ?? null), null); }
                    }
                }
            }
            return obj;
        }
    }
}