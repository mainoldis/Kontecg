using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kontecg.Reflection.Extensions
{
    public static class AssemblyExtensions
    {
        public static TAttribute GetSingleAttributeOrNull<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            Attribute[] attrs = assembly.GetCustomAttributes(typeof(TAttribute)).ToArray();
            if (attrs.Length > 0)
            {
                return (TAttribute) attrs[0];
            }

            return default;
        }

        /// <summary>
        ///     Gets directory path of given assembly or returns null if can not find.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static string GetDirectoryPathOrNull(this Assembly assembly)
        {
            string location = assembly.Location;

            DirectoryInfo directory = new FileInfo(location).Directory;

            return directory?.FullName;
        }
    }
}
