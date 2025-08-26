using System;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal static class DBNullHelper
    {
        public static bool IsNullableType(Type type)
        {
            return
                type.IsGenericType && !type.IsGenericTypeDefinition &&
                typeof(Nullable<>) == type.GetGenericTypeDefinition();
        }

        public static bool IsNull(object o)
        {
            return o == null || DBNull.Value.Equals(o);
        }

        public static object FromDb(object o)
        {
            return IsNull(o) ? null : o;
        }

        public static object ToDb(object o)
        {
            return IsNull(o) ? DBNull.Value : o;
        }
    }
}
