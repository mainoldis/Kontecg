using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Kontecg.Logging;
using Nito.Disposables;

namespace Kontecg.IO
{
    public sealed class LockingFileSystem : IDisposable
    {
        private IDisposable _handle;

        public LockingFileSystem(string key, TimeSpan timeOut)
        {
            string path = Path.Combine(Path.GetTempPath(), ".kontecg-lock-" + key);
            Stopwatch st = new Stopwatch();
            st.Start();

            FileStream fh = default(FileStream);

            while (st.Elapsed < timeOut)
            {
                try
                {
                    fh = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete);
                    fh.Write(new byte[] {0xba, 0xad, 0xf0, 0x0d}, 0, 4);
                    break;
                }
                catch (Exception ex)
                {
                    LogHelper.Logger.Warn($"Failed to grab lockfile, will retry: '{path}'", ex);
                    Thread.Sleep(250);
                }
            }

            st.Stop();

            if (fh == null)
            {
                throw new Exception("Couldn't acquire lock, is another instance running");
            }

            _handle = Disposable.Create(() =>
            {
                fh.Dispose();
                FileHelper.DeleteIfExists(path);
            });
        }

        public void Dispose()
        {
            IDisposable disposable = Interlocked.Exchange(ref _handle, null);
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        ~LockingFileSystem()
        {
            if (_handle == null)
            {
                return;
            }

            throw new AbandonedMutexException("Leaked a Mutex!");
        }
    }
}
