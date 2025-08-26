using Kontecg.Dependency;

namespace Kontecg.Data
{
    public interface IDatabaseCheckHelper : ITransientDependency
    {
        bool CanConnect(string connectionString);
    }
}
