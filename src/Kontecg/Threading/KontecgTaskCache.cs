using System.Threading.Tasks;

namespace Kontecg.Threading
{
    public static class KontecgTaskCache
    {
        public static Task CompletedTask { get; } = Task.FromResult(0);
    }
}
