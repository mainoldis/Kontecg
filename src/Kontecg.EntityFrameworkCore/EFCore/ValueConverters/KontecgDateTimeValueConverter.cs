using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Kontecg.Timing;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kontecg.EFCore.ValueConverters
{
    public class KontecgDateTimeValueConverter : ValueConverter<DateTime?, DateTime?>
    {
        private static readonly Expression<Func<DateTime?, DateTime?>> Normalize = x =>
            x.HasValue ? Clock.Normalize(x.Value) : x;

        public KontecgDateTimeValueConverter([CanBeNull] ConverterMappingHints mappingHints = null)
            : base(Normalize, Normalize, mappingHints)
        {
        }
    }
}
