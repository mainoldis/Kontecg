using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal static class EnumerableToDatatable
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            DataTable table = new(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type propType = prop.PropertyType;

                if (DBNullHelper.IsNullableType(propType))
                {
                    propType = new NullableConverter(propType).UnderlyingType;
                }

                table.Columns.Add(prop.Name, propType);
            }

            foreach (T item in items)
            {
                object[] values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }

            return table;
        }
    }
}
