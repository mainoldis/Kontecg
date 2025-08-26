using System.Threading.Tasks;
using Microsoft.ML;

namespace Kontecg.MachineLearning
{
    public interface IMlContextProvider
    {
        ValueTask<MLContext> GetContextAsync();

        MLContext GetContext();
    }
}
