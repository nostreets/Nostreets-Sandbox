using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace NostreetsORM.Interfaces
{
    public static class IDataReaderExt
    {
        public static string GetSafeString(this IDataReader reader, Int32 ordinal, bool trim = true)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                if(trim)
                    return reader.GetString(ordinal).Trim();
                else
                    return reader.GetString(ordinal);


            }

            else
            {
                return null;
            }
        }

        public static Guid? GetSafeGuid(this IDataReader reader, Int32 ordinal, Guid? defaultValue = null)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetGuid(ordinal);
            }

            else
            {
                return defaultValue;
            }



        }

        public static Guid GetSafeGuid(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetGuid(ordinal);
            }

            else
            {
                return Guid.Empty;
            }



        }

        public static Uri GetSafeUri(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                string sUri = reader.GetString(ordinal);
                Uri created = null;

                if (Uri.TryCreate(sUri, UriKind.RelativeOrAbsolute, out created))
                {
                    return created;
                }
                return null;
            }

            else
            {
                return null;
            }
        }

        public static int GetSafeInt32(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetInt32(ordinal);
            }
            else
            {
                return 0;
            }
        }

        public static int? GetSafeInt32(this IDataReader reader, Int32 ordinal, Int32? defaultValue = null)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetInt32(ordinal);
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool GetSafeBool(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetBoolean(ordinal);
            }
            else
            {
                return false;
            }
        }

        public static bool? GetSafeBoolNullable(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetBoolean(ordinal);
            }
            else
            {
                return null;
            }
        }

        public static Int16 GetSafeInt16(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetInt16(ordinal);
            }
            else
            {
                return 0;
            }
        }

        public static DateTime GetSafeDateTime(this IDataReader reader, Int32 ordinal, DateTimeKind kind = DateTimeKind.Utc)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return DateTime.SpecifyKind(reader.GetDateTime(ordinal), kind);
            }
            else
            {
                return  DateTime.MinValue;
            }
        }

        public static SqlBytes GetSafeSqlBytes(this SqlDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetSqlBytes(ordinal);
            }
            else
            {
                return new SqlBytes();
            }
        }

        public static double? GetSafeDouble(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetDouble(ordinal);
            }
            else
            {
                return null;
            }
        }

        public static decimal? GetSafeDecimal(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetDecimal(ordinal);
            }
            else
            {
                return null;
            }
        }

        public static byte? GetSafeByte(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetByte(ordinal);
            }
            else
            {
                return null;
            }
        }

        public static float? GetSafeFloat(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetFloat(ordinal);
            }
            else
            {
                return null;
            }
        }

        public static object GetSafeValue(this IDataReader reader, Int32 ordinal)
        {
            if (reader[ordinal] != null && reader[ordinal] != DBNull.Value)
            {
                return reader.GetValue(ordinal);
            }
            else
            {
                return null;
            }
        }

        public static TEnum GetSafeEnum<TEnum>(this IDataReader reader, Int32 ordinal) where TEnum : struct
        {
            string enumValue = null;
            object value = reader.GetSafeValue(ordinal);
           
            TEnum parsedValue = default(TEnum);

            if (value == null)
                return parsedValue;
            else
                enumValue = value.ToString();

            


            if (!String.IsNullOrEmpty(enumValue) && Enum.TryParse(enumValue, out parsedValue))
            {
                if (!Enum.IsDefined(typeof(TEnum), parsedValue))
                {
                    parsedValue = default(TEnum);
                }
            }

            return parsedValue;
        }


    }
}
