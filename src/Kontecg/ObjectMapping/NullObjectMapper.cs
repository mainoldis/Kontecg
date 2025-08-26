using System.Linq;
using Kontecg.Dependency;

namespace Kontecg.ObjectMapping
{
    public sealed class NullObjectMapper : IObjectMapper, ISingletonDependency
    {
        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullObjectMapper Instance { get; } = new();

        public TDestination Map<TDestination>(object source)
        {
            throw new KontecgException(
                "Kontecg.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            throw new KontecgException(
                "Kontecg.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        {
            throw new KontecgException(
                "Kontecg.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }
    }
}
