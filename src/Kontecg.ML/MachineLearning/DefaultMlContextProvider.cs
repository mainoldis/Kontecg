using System.Threading.Tasks;
using Microsoft.ML;

namespace Kontecg.MachineLearning
{
    public sealed class DefaultMlContextProvider : IMlContextProvider
    {
        public DefaultMlContextProvider()
        {
            MlContext = KontecgMlContextFactory.Create();
        }

        public MLContext MlContext { get; }

        public ValueTask<MLContext> GetContextAsync()
        {
            return ValueTask.FromResult(MlContext);
        }

        public MLContext GetContext()
        {
            return MlContext;
        }
    }
}
