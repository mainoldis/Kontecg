namespace Kontecg.EFCore
{
    public interface IShouldInitializeDbContext
    {
        void Initialize(KontecgEfDbContextInitializationContext initializationContext);
    }
}
