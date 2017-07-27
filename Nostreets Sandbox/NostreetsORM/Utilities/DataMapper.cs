using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NostreetsORM.Utilities
{
    public class DataMapper<T>
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

            if (props.Length > 1)
            {
                foreach (var prop in props)
                {
                    if (colname.Contains(prop.Name.ToLower()))
                    {
                        Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (type.Name != "String" && type.Name != "Char" && type.IsClass)
                        {
                            object property = JsonConvert.DeserializeObject(reader.GetString(reader.GetOrdinal(prop.Name)) ?? "", type);
                            if (property == null) { property = Activator.CreateInstance(type); }
                            if (property.GetType() != type) { throw new Exception("Property " + type.Name + " Does Not Equal Type in DB"); }
                            prop.SetValue(obj, property, null);
                        }
                        else if (reader[prop.Name] != DBNull.Value)
                        {
                            if (reader[prop.Name].GetType() == typeof(decimal)) { prop.SetValue(obj, (reader.GetDouble(prop.Name)), null); }
                            else { prop.SetValue(obj, (reader.GetValue(reader.GetOrdinal(prop.Name)) ?? null), null); }
                        }
                    }
                }
            }
            else
            {
                var item = reader.GetValue(reader.GetSchemaTable().Columns[0].Ordinal);
                if (item != null) { obj = (T)item; }
            }

            return obj;
        }
    }
}