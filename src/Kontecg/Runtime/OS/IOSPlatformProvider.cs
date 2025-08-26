using System.Runtime.InteropServices;

namespace Kontecg.Runtime.OS
{
    public interface IOSPlatformProvider
    {
        OSPlatform GetCurrentOSPlatform();
    }
}
