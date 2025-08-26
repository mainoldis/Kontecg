using System;
using JetBrains.Annotations;
using Kontecg.Application.Services;
using Kontecg.Collections.Extensions;

namespace Kontecg.Aspects
{
    internal static class KontecgCrossCuttingConcerns
    {
        public const string Auditing = "KontecgAuditing";
        public const string Validation = "KontecgValidation";
        public const string UnitOfWork = "KontecgUnitOfWork";
        public const string Authorization = "KontecgAuthorization";
        public const string ExceptionHandling = "KontecgExceptionHandling";

        public static void AddApplied(object obj, params string[] concerns)
        {
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.AddRange(concerns);
        }

        public static void RemoveApplied(object obj, params string[] concerns)
        {
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            if (!(obj is IAvoidDuplicateCrossCuttingConcerns crossCuttingEnabledObj))
            {
                return;
            }

            foreach (string concern in concerns)
            {
                crossCuttingEnabledObj.AppliedCrossCuttingConcerns.RemoveAll(c => c == concern);
            }
        }

        public static bool IsApplied([NotNull] object obj, [NotNull] string concern)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (concern == null)
            {
                throw new ArgumentNullException(nameof(concern));
            }

            return (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.Contains(concern) ?? false;
        }

        public static IDisposable Applying(object obj, params string[] concerns)
        {
            AddApplied(obj, concerns);
            return new DisposeAction(() => { RemoveApplied(obj, concerns); });
        }

        public static string[] GetAppliedTo(object obj)
        {
            return !(obj is IAvoidDuplicateCrossCuttingConcerns crossCuttingEnabledObj)
                ? new string[0]
                : crossCuttingEnabledObj.AppliedCrossCuttingConcerns.ToArray();
        }
    }
}
