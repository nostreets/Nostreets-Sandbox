using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Utilities
{
    public static class Extend
    {
        public static DataTable ToDataTable<T>(this List<T> iList)
        {
            DataTable dataTable = new DataTable();
            //PropertyDescriptorCollection
              List<PropertyDescriptor> propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T)).Cast<PropertyDescriptor>()
                .Where((a) => a.Name !=  "sCarrier_Method_Desc" && a.Name != "dtFulfilled_DT" ).ToList();
            foreach (PropertyDescriptor item in propertyDescriptorCollection)
            {
               
                    PropertyDescriptor propertyDescriptor = item;
                    Type type = propertyDescriptor.PropertyType;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = Nullable.GetUnderlyingType(type);

                    dataTable.Columns.Add(propertyDescriptor.Name, type); 
                
            }

            //new object[propertyDescriptorCollection.Count];
            int id = 0;
            foreach (T iListItem in iList)
            {
                ArrayList values = new ArrayList();
                for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                {
                    values.Add(propertyDescriptorCollection[i].GetValue(iListItem) == null
                        && propertyDescriptorCollection[i].PropertyType == typeof(string)
                        ? String.Empty
                        : (i == 0)
                        ? id += 1
                        : propertyDescriptorCollection[i].GetValue(iListItem));
                    
                    //values[i] = (propertyDescriptorCollection[i].GetValue(iListItem).GetType() != typeof(string)) ? propertyDescriptorCollection[i].GetValue(iListItem) : DBNull.Value; 
                }
                dataTable.Rows.Add(values.ToArray());
             
                values = null;
            }
            return dataTable;
        }

        public static object HitEndpoint(this object obj, string url, string method = "GET", object data = null, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            HttpWebRequest requestStream = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse responseStream = null;
            string responseString, requestString;

            requestStream.ContentType = contentType;
            requestStream.Method = method;

            foreach (KeyValuePair<string, string> head in headers) {
                requestStream.Headers.Add(head.Key, head.Value);
            }

            try
            {
                if (data != null)
                {
                    if (method == "POST" || method == "PUT" || method == "PATCH")
                    {
                        requestString = JsonConvert.SerializeObject(data);
                        using (Stream stream = requestStream.GetRequestStream())
                        {
                            StreamWriter writer = new StreamWriter(stream);
                            writer.Write(requestString);
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            try
            {
                using (responseStream = (HttpWebResponse)requestStream.GetResponse())
                {
                    using (Stream stream = responseStream.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        responseString = reader.ReadToEnd();
                    }
                    Dictionary<string, dynamic> responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static T HitEndpoint<T>(this object obj, string url, string method = "GET", object data = null, string contentType = "application/json", Dictionary<string, string> headers = null) {
            HttpWebRequest requestStream = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse responseStream = null;
            string responseString, requestString;

            requestStream.ContentType = contentType;
            requestStream.Method = method;

            foreach (KeyValuePair<string, string> head in headers)
            {
                requestStream.Headers.Add(head.Key, head.Value);
            }

            try
            {
                if (data != null)
                {
                    if (method == "POST" || method == "PUT" || method == "PATCH")
                    {
                        requestString = JsonConvert.SerializeObject(data);
                        using (Stream stream = requestStream.GetRequestStream())
                        {
                            StreamWriter writer = new StreamWriter(stream);
                            writer.Write(requestString);
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                using (responseStream = (HttpWebResponse)requestStream.GetResponse())
                {
                    using (Stream stream = responseStream.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        responseString = reader.ReadToEnd();
                    }
                    T responseData = JsonConvert.DeserializeObject<T>(responseString);
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        public static int GetWeekOfYear(this DateTime time)
        {
            GregorianCalendar _gc = new GregorianCalendar();
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static bool IsWeekOfMonth(this int week, DateTime payDay)
        {
            bool result = false;
            int weekOfPay = GetWeekOfMonth(payDay);

            while (week >= weekOfPay)
            {
                week -= 4;
                if (week == weekOfPay) { result = true; }
            }

            return result;

        }

        public static DateTime ToDateTime(this string obj, string format = null)
        {
            if (format != null) { return DateTime.ParseExact(obj, format, CultureInfo.InvariantCulture); }
            else { return Convert.ToDateTime(obj); }
        }

        public static void AddAttribute<T>(this object obj, bool affectThisObj = true, Dictionary<object, Type> attributeParams = null, FieldInfo[] affectedFields = null) where T : Attribute
        {
            Type type = obj.GetType();

            AssemblyName aName = new AssemblyName("SomeNamespace");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);
            TypeBuilder affectedType = moduleBuilder.DefineType(type.Name + "Proxy", TypeAttributes.Public, type);


            Type[] attrParams = attributeParams.Values.ToArray();
            ConstructorInfo attrConstructor = typeof(T).GetConstructor(attrParams);
            CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(attrConstructor, attributeParams.Keys.ToArray());


            if (affectThisObj)
            {
                affectedType.SetCustomAttribute(attrBuilder);
            }
            else if (affectedFields != null && affectedFields.Length > 1)
            {
                foreach (FieldInfo field in affectedFields)
                {
                    FieldBuilder firstNameField = affectedType.DefineField(field.Name, field.FieldType, (field.IsPrivate) ? FieldAttributes.Private : FieldAttributes.Public);
                    firstNameField.SetCustomAttribute(attrBuilder);
                }
            }


            Type newType = affectedType.CreateType();
            object instance = Activator.CreateInstance(newType);


            obj = instance;

        }

    }
}
