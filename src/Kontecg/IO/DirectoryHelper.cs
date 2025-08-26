using System;
using System.IO;
using System.Threading.Tasks;
using Kontecg.Collections.Extensions;
using Kontecg.Logging;

namespace Kontecg.IO
{
    /// <summary>
    ///     A helper class for Directory operations.
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        ///     Creates a new directory if it does not exists.
        /// </summary>
        /// <param name="directoryPath">Directory to create</param>
        public static void CreateIfNotExists(string directoryPath)
        {
            Check.NotNullOrWhiteSpace(directoryPath, nameof(directoryPath));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        ///     Delete a directory if it does exists.
        /// </summary>
        /// <param name="directoryPath">Directory to delete</param>
        public static async Task DeleteIfExistsAsync(string directoryPath)
        {
            Check.NotNullOrWhiteSpace(directoryPath, nameof(directoryPath));
            if (!Directory.Exists(directoryPath))
            {
                return;
            }
            //From http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true/329502#329502

            string[] files = Array.Empty<string>();
            try
            {
                files = Directory.GetFiles(directoryPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogHelper.Logger.Warn($"The files inside '{directoryPath}' could not be read", ex);
            }

            string[] dirs = Array.Empty<string>();
            try
            {
                dirs = Directory.GetDirectories(directoryPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogHelper.Logger.Warn($"The directories inside '{directoryPath}' could not be read", ex);
            }

            Task filesOps = files.ForEachAsync(f =>
            {
                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);
            });

            Task dirOps = dirs.ForEachAsync(DeleteIfExistsAsync);

            await Task.WhenAll(filesOps, dirOps);

            LogHelper.Logger.Debug($"Now deleting folder '{directoryPath}'");
            File.SetAttributes(directoryPath, FileAttributes.Normal);

            try
            {
                Directory.Delete(directoryPath, false);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error($"Could not delete '{directoryPath}'", ex);
            }
        }
    }
}
