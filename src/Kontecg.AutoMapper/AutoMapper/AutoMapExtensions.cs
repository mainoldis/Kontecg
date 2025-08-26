using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Kontecg.Localization;

namespace Kontecg.AutoMapper
{
    public static class AutoMapExtensions
    {
        private const int MaxCultureFallbackDepth = 5;

        public static CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination>
            CreateMultiLingualMap<TMultiLingualEntity, TMultiLingualEntityPrimaryKey, TTranslation, TDestination>(
                this IMapperConfigurationExpression configuration, MultiLingualMapContext multiLingualMapContext,
                bool fallbackToParentCultures = false)
            where TTranslation : class, IEntityTranslation<TMultiLingualEntity, TMultiLingualEntityPrimaryKey>
            where TMultiLingualEntity : IMultiLingualEntity<TTranslation>
        {
            CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination> result =
                new CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination>
                {
                    TranslationMap = configuration.CreateMap<TTranslation, TDestination>(),
                    EntityMap = configuration.CreateMap<TMultiLingualEntity, TDestination>().BeforeMap(
                        (source, destination, context) =>
                        {
                            if (source.Translations.IsNullOrEmpty())
                            {
                                return;
                            }

                            TTranslation translation =
                                source.Translations.FirstOrDefault(pt =>
                                    pt.Language == CultureInfo.CurrentUICulture.Name);
                            if (translation != null)
                            {
                                context.Mapper.Map(translation, destination);
                                return;
                            }

                            if (fallbackToParentCultures)
                            {
                                translation =
                                    GeTranslationBasedOnCulturalRecursive<TMultiLingualEntity,
                                        TMultiLingualEntityPrimaryKey
                                        ,
                                        TTranslation>(CultureInfo.CurrentUICulture.Parent, source.Translations, 0);
                                if (translation != null)
                                {
                                    context.Mapper.Map(translation, destination);
                                    return;
                                }
                            }

                            string defaultLanguage =
                                multiLingualMapContext.SettingManager.GetSettingValue(LocalizationSettingNames
                                    .DefaultLanguage);

                            translation = source.Translations.FirstOrDefault(pt => pt.Language == defaultLanguage);
                            if (translation != null)
                            {
                                context.Mapper.Map(translation, destination);
                                return;
                            }

                            translation = source.Translations.FirstOrDefault();
                            if (translation != null)
                            {
                                context.Mapper.Map(translation, destination);
                            }
                        })
                };


            return result;
        }

        public static CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination>
            CreateMultiLingualMap<TMultiLingualEntity, TTranslation, TDestination>(
                this IMapperConfigurationExpression configuration,
                MultiLingualMapContext multiLingualMapContext,
                bool fallbackToParentCultures = false)
            where TTranslation : class, IEntity, IEntityTranslation<TMultiLingualEntity, int>
            where TMultiLingualEntity : IMultiLingualEntity<TTranslation>
        {
            return configuration.CreateMultiLingualMap<TMultiLingualEntity, int, TTranslation, TDestination>(
                multiLingualMapContext, fallbackToParentCultures);
        }

        private static TTranslation GeTranslationBasedOnCulturalRecursive<TMultiLingualEntity,
            TMultiLingualEntityPrimaryKey, TTranslation>(CultureInfo culture, ICollection<TTranslation> translations,
            int currentDepth)
            where TTranslation : class, IEntityTranslation<TMultiLingualEntity, TMultiLingualEntityPrimaryKey>
        {
            if (culture == null || culture.Name.IsNullOrWhiteSpace() || translations.IsNullOrEmpty() ||
                currentDepth > MaxCultureFallbackDepth)
            {
                return null;
            }

            TTranslation translation = translations.FirstOrDefault(pt =>
                pt.Language.Equals(culture.Name, StringComparison.OrdinalIgnoreCase));
            return translation ??
                   GeTranslationBasedOnCulturalRecursive<TMultiLingualEntity, TMultiLingualEntityPrimaryKey,
                       TTranslation>(culture.Parent, translations, currentDepth + 1);
        }
    }
}
