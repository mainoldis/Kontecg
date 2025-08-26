using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Kontecg.Reflection;
using Kontecg.Timing;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Utils
{
    internal static class DateTimePropertyInfoHelper
    {
        /// <summary>
        ///     Key: Entity type
        ///     Value: DateTime property infos
        /// </summary>
        private static readonly ConcurrentDictionary<Type, EntityDateTimePropertiesInfo> DateTimeProperties;

        static DateTimePropertyInfoHelper()
        {
            DateTimeProperties = new ConcurrentDictionary<Type, EntityDateTimePropertiesInfo>();
        }

        public static EntityDateTimePropertiesInfo GetDatePropertyInfos(Type entityType)
        {
            return DateTimeProperties.GetOrAdd(
                entityType,
                key => FindDatePropertyInfosForType(entityType)
            );
        }

        public static void NormalizeDatePropertyKinds(object entity, Type entityType)
        {
            if (entityType.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
            {
                return;
            }

            EntityDateTimePropertiesInfo dateTimePropertyInfos = GetDatePropertyInfos(entityType);

            dateTimePropertyInfos.DateTimePropertyInfos.ForEach(property =>
            {
                DisableDateTimeNormalizationAttribute attribute = ReflectionHelper
                    .GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(
                        property
                    );

                if (attribute != null)
                {
                    return;
                }

                DateTime? dateTime = (DateTime?) property.GetValue(entity);
                if (dateTime.HasValue)
                {
                    property.SetValue(entity, Clock.Normalize(dateTime.Value));
                }
            });

            dateTimePropertyInfos.ComplexTypePropertyPaths.ForEach(propertyPath =>
            {
                PropertyInfo property =
                    (PropertyInfo) ReflectionHelper.GetPropertyByPath(entity, entityType, propertyPath);

                DisableDateTimeNormalizationAttribute attribute = ReflectionHelper
                    .GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(
                        property
                    );

                if (attribute != null)
                {
                    return;
                }

                DateTime? dateTime = (DateTime?) ReflectionHelper.GetValueByPath(entity, entityType, propertyPath);
                if (dateTime.HasValue)
                {
                    ReflectionHelper.SetValueByPath(entity, entityType, propertyPath, Clock.Normalize(dateTime.Value));
                }
            });
        }

        /// <summary>
        ///     Gets DateTime properties which is not marked with <see cref="DisableDateTimeNormalizationAttribute" />
        ///     or it's parent is not marked with <see cref="DisableDateTimeNormalizationAttribute" />
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static EntityDateTimePropertiesInfo FindDatePropertyInfosForType(Type entityType)
        {
            List<PropertyInfo> datetimeProperties = entityType.GetProperties()
                .Where(property =>
                    (property.PropertyType == typeof(DateTime) ||
                     property.PropertyType == typeof(DateTime?)) &&
                    property.CanWrite &&
                    !property.IsDefined(typeof(NotMappedAttribute)) &&
                    !property.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true)
                ).ToList();

            List<PropertyInfo> complexTypeProperties = entityType.GetProperties()
                .Where(p =>
                    p.PropertyType.IsDefined(typeof(OwnedAttribute), true) &&
                    !p.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true)
                )
                .ToList();

            List<string> complexTypeDateTimePropertyPaths = new List<string>();
            foreach (PropertyInfo complexTypeProperty in complexTypeProperties)
            {
                AddComplexTypeDateTimePropertyPaths(entityType.FullName + "." + complexTypeProperty.Name,
                    complexTypeProperty, complexTypeDateTimePropertyPaths);
            }

            return new EntityDateTimePropertiesInfo
            {
                DateTimePropertyInfos = datetimeProperties,
                ComplexTypePropertyPaths = complexTypeDateTimePropertyPaths
            };
        }

        private static void AddComplexTypeDateTimePropertyPaths(string pathPrefix, PropertyInfo complexProperty,
            List<string> complexTypeDateTimePropertyPaths)
        {
            if (!complexProperty.PropertyType.IsDefined(typeof(OwnedAttribute), true))
            {
                return;
            }

            List<string> complexTypeDateProperties = complexProperty.PropertyType
                .GetProperties()
                .Where(property =>
                    (property.PropertyType == typeof(DateTime) ||
                     property.PropertyType == typeof(DateTime?)) &&
                    property.CanWrite &&
                    !property.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true)
                ).Select(p => pathPrefix + "." + p.Name).ToList();

            complexTypeDateTimePropertyPaths.AddRange(complexTypeDateProperties);

            List<PropertyInfo> complexTypeProperties = complexProperty.PropertyType.GetProperties()
                .Where(p =>
                    p.PropertyType.IsDefined(typeof(OwnedAttribute), true) &&
                    !p.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true)
                )
                .ToList();

            if (!complexTypeProperties.Any())
            {
                return;
            }

            foreach (PropertyInfo complexTypeProperty in complexTypeProperties)
            {
                AddComplexTypeDateTimePropertyPaths(pathPrefix + "." + complexTypeProperty.Name, complexTypeProperty,
                    complexTypeDateTimePropertyPaths);
            }
        }
    }
}
