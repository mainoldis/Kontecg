using System.Collections.Generic;
using System.Linq;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Used to store <see cref="Feature" />s.
    /// </summary>
    public class FeatureDictionary : Dictionary<string, Feature>
    {
        /// <summary>
        ///     Adds all the child features of the current features, recursively.
        /// </summary>
        public void AddAllFeatures()
        {
            foreach (Feature feature in Values.ToList())
            {
                AddFeatureRecursively(feature);
            }
        }

        private void AddFeatureRecursively(Feature feature)
        {
            //Prevent multiple additions of the same-named feature.
            if (TryGetValue(feature.Name, out Feature existingFeature))
            {
                if (existingFeature != feature)
                {
                    throw new KontecgInitializationException("Duplicate feature name detected for " + feature.Name);
                }
            }
            else
            {
                this[feature.Name] = feature;
            }

            //Add child features (recursive call)
            foreach (Feature childFeature in feature.Children)
            {
                AddFeatureRecursively(childFeature);
            }
        }
    }
}
