using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal static class DataReaderExtensions
    {
        public static IEnumerable<IDataRecord> AsEnumerable(this IDataReader reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<dynamic> ToDynamic(this IEnumerable<IDataRecord> input)
        {
            return from item in input select item.ToExpando();
        }

        public static IEnumerable<IEnumerable<dynamic>> ToMultiResultSet(this IDataReader reader)
        {
            do
            {
                yield return GetResultSet(reader);
            } while (reader.NextResult());
        }

        private static IEnumerable<dynamic> GetResultSet(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return reader.ToExpando();
            }
        }
    }
}
