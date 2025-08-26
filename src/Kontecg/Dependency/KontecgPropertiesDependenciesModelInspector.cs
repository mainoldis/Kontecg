using Castle.Core;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.SubSystems.Conversion;

namespace Kontecg.Dependency
{
    public class KontecgPropertiesDependenciesModelInspector : PropertiesDependenciesModelInspector
    {
        public KontecgPropertiesDependenciesModelInspector(IConversionManager converter)
            : base(converter)
        {
        }

        protected override void InspectProperties(ComponentModel model)
        {
            if (model.Implementation.FullName != null &&
                model.Implementation.FullName.StartsWith("Microsoft"))
            {
                return;
            }

            base.InspectProperties(model);
        }
    }
}
