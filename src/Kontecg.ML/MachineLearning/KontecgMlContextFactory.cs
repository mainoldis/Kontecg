using Microsoft.ML;

namespace Kontecg.MachineLearning
{
    internal static class KontecgMlContextFactory
    {
        public static MLContext Create()
        {
            return new MLContext(RandomHelper.GetRandom());
        }
    }
}
