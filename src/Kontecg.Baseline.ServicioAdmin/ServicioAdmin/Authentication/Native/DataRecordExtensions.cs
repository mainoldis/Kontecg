using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal static class DataRecordExtensions
    {
        // stolen from Massive
        /// <summary>
        ///     Convert a datarecord into a dynamic object, so that properties can be simply accessed
        ///     using standard C# syntax.
        /// </summary>
        /// <param name="rdr">the data record</param>
        /// <returns>A dynamic object with fields corresponding to the database columns</returns>
        public static dynamic ToExpando(this IDataRecord rdr)
        {
            dynamic e = new ExpandoObject();
            IDictionary<string, object> d = e as IDictionary<string, object>;
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                string name = rdr.GetName(i);
                object value = rdr[i];
                d.Add(name, DBNullHelper.FromDb(value));
            }

            return e;
        }

        /// <summary>
        ///     Get a value from an IDataRecord by column name. This method supports all types,
        ///     as long as the DbType is convertible to the CLR Type passed as a generic argument.
        ///     Also handles conversion from DbNull to null, including nullable types.
        /// </summary>
        public static TResult Get<TResult>(this IDataRecord reader, string name)
        {
            return reader.Get<TResult>(reader.GetOrdinal(name));
        }

        /// <summary>
        ///     Get a value from an IDataRecord by index. This method supports all types,
        ///     as long as the DbType is convertible to the CLR Type passed as a generic argument.
        ///     Also handles conversion from DbNull to null, including nullable types.
        /// </summary>
        public static TResult Get<TResult>(this IDataRecord reader, int c)
        {
            return ConvertTo<TResult>.From(reader[c]);
        }
    }
}
