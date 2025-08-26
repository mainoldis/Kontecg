namespace Kontecg.MachineLearning.Configuration
{
    public class KontecgMachineLearningConfiguration : IKontecgMachineLearningConfiguration
    {
        public bool IsEnabled { get; private set; }

        public void Enable()
        {
            IsEnabled = true;
        }
    }
}
