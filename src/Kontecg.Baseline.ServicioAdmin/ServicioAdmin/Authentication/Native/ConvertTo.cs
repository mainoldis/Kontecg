using System;
using System.Reflection;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal static class ConvertTo<T>
    {
        // ReSharper disable StaticFieldInGenericType
        // clearly we *want* a static field for each instantiation of this generic class...
        /// <summary>
        ///     The actual conversion method. Converts an object to any type using standard casting functionality, taking into
        ///     account null/nullable types
        ///     and avoiding DBNull issues. This method is set as a delegate at runtime (in the static constructor).
        /// </summary>
        public static readonly Func<object, T> From;
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        ///     Set the <see cref="From" /> delegate, depending on whether T is a reference type, a nullable value type or a value
        ///     type.
        /// </summary>
        static ConvertTo()
        {
            From = CreateConvertFunction(typeof(T));
        }

        private static Func<object, T> CreateConvertFunction(Type type)
        {
            if (!type.IsValueType)
            {
                return ConvertRefType;
            }

            if (DBNullHelper.IsNullableType(type))
            {
                Type delegateType = typeof(Func<object, T>);
                MethodInfo methodInfo = typeof(ConvertTo<T>).GetMethod("ConvertNullableValueType",
                    BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo genericMethodForElement = methodInfo.MakeGenericMethod(type.GetGenericArguments()[0]);
                return (Func<object, T>) Delegate.CreateDelegate(delegateType, genericMethodForElement);
            }

            return ConvertValueType;
        }

        // ReSharper disable UnusedMember.Local
        // (used via reflection!)
        private static TElem? ConvertNullableValueType<TElem>(object value) where TElem : struct
        {
            return DBNullHelper.IsNull(value) ? null : ConvertPrivate<TElem>(value);
        }
        // ReSharper restore UnusedMember.Local

        private static T ConvertRefType(object value)
        {
            return DBNullHelper.IsNull(value) ? default : ConvertPrivate<T>(value);
        }

        private static T ConvertValueType(object value)
        {
            if (DBNullHelper.IsNull(value))
            {
                throw new NullReferenceException("Value is DbNull");
            }

            return ConvertPrivate<T>(value);
        }

        private static TElem ConvertPrivate<TElem>(object value)
        {
            return (TElem) Convert.ChangeType(value, typeof(TElem));
        }
    }
}
