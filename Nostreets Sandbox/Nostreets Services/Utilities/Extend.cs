using Newtonsoft.Json;
using NostreetsORM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using System.Net.Http.Headers;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace Nostreets_Services.Utilities
{
    public static class Extend
    {
        public static DataTable ToDataTable<T>(this List<T> iList)
        {
            DataTable dataTable = new DataTable();
            //PropertyDescriptorCollection
            List<PropertyDescriptor> propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T)).Cast<PropertyDescriptor>()
              .Where((a) => a.Name != "sCarrier_Method_Desc" && a.Name != "dtFulfilled_DT").ToList();
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

        public static DateTime ToDateTime(this string obj, string format = null)
        {
            if (format != null) { return DateTime.ParseExact(obj, format, CultureInfo.InvariantCulture); }
            else { return Convert.ToDateTime(obj); }
        }

        public static object HitEndpoint(this BaseService obj, string url, string method = "GET", object data = null, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
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
                        if (contentType == "application/json")
                        {
                            requestString = JsonConvert.SerializeObject(data);


                        }
                        else
                        {
                            XmlSerializer serial = new XmlSerializer(data.GetType());
                            StringWriter writer = new StringWriter();
                            serial.Serialize(writer, data);
                            requestString = writer.ToString();
                            writer.Close();
                        }

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

                    object responseData;

                    if (contentType == "application/json")
                    {
                        responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);
                    }
                    else
                    {
                        XmlSerializer serial = new XmlSerializer(data.GetType());
                        StringReader reader = new StringReader(responseString);
                        responseData = serial.Deserialize(reader);
                    }
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static T HitEndpoint<T>(this BaseService obj, string url, string method = "GET", object data = null, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            HttpWebRequest requestStream = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse responseStream = null;
            string responseString = null,
                   requestString = null;
            byte[] bytes = null;

            if (headers == null) { headers = new Dictionary<string, string>(); }

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
                        if (contentType == "application/json")
                        {
                            requestString = JsonConvert.SerializeObject(data);
                        }
                        else if (contentType == "text/xml; encoding='utf-8'")
                        {
                            XmlSerializer serial = new XmlSerializer(data.GetType());
                            StringWriter writer = new StringWriter();
                            serial.Serialize(writer, data);
                            requestString = "XML=" + writer.ToString();
                            writer.Close();
                        }
                    }

                    using (Stream stream = requestStream.GetRequestStream())
                    {
                        StreamWriter writer = new StreamWriter(stream);
                        if (requestString != null) { writer.Write(requestString); }
                        else if (bytes != null) { stream.Write(bytes, 0, bytes.Length); }
                        writer.Close();
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

                    T responseData;

                    if (contentType == "application/json")
                    {
                        responseData = JsonConvert.DeserializeObject<T>(responseString);
                    }
                    else
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(T));
                        StringReader reader = new StringReader(responseString); //XmlReader.Create(responseString);
                        responseData = (T)serial.Deserialize(reader);
                    }

                    if (responseString.ToLower().Contains("<error>"))
                    { throw new Exception(responseString); }

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

        public static void RunPowerShellCommand(this string command, params string[] parameters)
        {
            string script = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted; Get-ExecutionPolicy"; // the second command to know the ExecutionPolicy level

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript(script);
                var someResult = powershell.Invoke();


                powershell.AddCommand(command);
                powershell.AddParameters(parameters);
                var results = powershell.Invoke();
            }
        }

        public static void AddAttribute<T>(this object obj, bool affectBaseObj = true, Dictionary<object, Type> attributeParams = null, FieldInfo[] affectedFields = null) where T : Attribute
        {
            Type type = obj.GetType();

            AssemblyName aName = new AssemblyName("SomeNamespace");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);
            TypeBuilder affectedType = moduleBuilder.DefineType(type.Name + "Proxy", TypeAttributes.Public, type);


            Type[] attrParams = attributeParams.Values.ToArray();
            ConstructorInfo attrConstructor = typeof(T).GetConstructor(attrParams);
            CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(attrConstructor, attributeParams.Keys.ToArray());


            if (affectBaseObj)
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

        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;
        }

        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        public static Dictionary<int, string> ToDictionary<T>(this Type @enum) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum");

            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(t => (int)(object)t, t => t.ToString());
        }

        public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs()
                          .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }

        public static string GetQueryString(this HttpRequestMessage request, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, key, true) == 0);
            if (string.IsNullOrEmpty(match.Value))
                return null;

            return match.Value;
        }

        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }

        public static string GetCookie(this HttpRequestMessage request, string cookieName)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
            {
                return cookie[cookieName].Value;
            }

            return null;
        }

        public static void SetCookie(this HttpRequestMessage request, ref HttpResponseMessage response, string cookieName, string value, DateTimeOffset? expires = null)
        {
            try
            {
                CookieHeaderValue storedCookie = request.Headers.GetCookies(cookieName).FirstOrDefault();

                if (storedCookie != null)
                {
                    storedCookie.Expires = expires;
                    storedCookie[cookieName].Value = value;
                }
                else
                {
                    CookieHeaderValue currentCookie = new CookieHeaderValue(cookieName, value);
                    currentCookie.Expires = expires;

                    response.Headers.AddCookies(new CookieHeaderValue[] { currentCookie });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void SetCookie(this HttpRequestMessage request, ref HttpResponseMessage response, string cookieName, Dictionary<string, string> values, DateTimeOffset? expires = null)
        {
            try
            {
                CookieHeaderValue storedCookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
                if (storedCookie != null)
                {
                    storedCookie.Expires = expires;

                    foreach (var item in values)
                    {
                        storedCookie[item.Key].Value = item.Value;
                    }
                }
                else
                {
                    CookieHeaderValue currentCookie = new CookieHeaderValue(cookieName, values.ToNameValueCollection());
                    currentCookie.Expires = expires;

                    response.Headers.AddCookies(new CookieHeaderValue[] { currentCookie });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static Delegate ToDelegate(this MethodInfo obj, object target = null)
        {

            Type delegateType;

            var typeArgs = obj.GetParameters()
                .Select(p => p.ParameterType)
                .ToList();

            // builds a delegate type
            if (obj.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(typeArgs.ToArray());

            }
            else
            {
                typeArgs.Add(obj.ReturnType);
                delegateType = Expression.GetFuncType(typeArgs.ToArray());
            }

            // creates a binded delegate if target is supplied
            var result = (target == null)
                ? Delegate.CreateDelegate(delegateType, obj)
                : Delegate.CreateDelegate(delegateType, target, obj);

            return result;
        }

        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in dict)
            {
                string value = null;
                if (kvp.Value != null)
                    value = kvp.Value.ToString();

                nameValueCollection.Add(kvp.Key.ToString(), value);
            }

            return nameValueCollection;
        }
    }
}
