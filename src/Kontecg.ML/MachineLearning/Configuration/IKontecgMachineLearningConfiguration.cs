namespace Kontecg.MachineLearning.Configuration
{
    public interface IKontecgMachineLearningConfiguration
    {
        bool IsEnabled { get; }

        void Enable();
    }
}
