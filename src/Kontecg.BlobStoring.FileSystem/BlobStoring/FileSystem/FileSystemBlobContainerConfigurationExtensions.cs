﻿using System;

namespace Kontecg.BlobStoring.FileSystem
{
    public static class FileSystemBlobContainerConfigurationExtensions
    {
        public static FileSystemBlobProviderConfiguration GetFileSystemConfiguration(
            this BlobContainerConfiguration containerConfiguration)
        {
            return new FileSystemBlobProviderConfiguration(containerConfiguration);
        }

        public static BlobContainerConfiguration UseFileSystem(
            this BlobContainerConfiguration containerConfiguration,
            Action<FileSystemBlobProviderConfiguration> fileSystemConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(FileSystemBlobProvider);
            containerConfiguration.NamingNormalizers.TryAdd<FileSystemBlobNamingNormalizer>();

            fileSystemConfigureAction(new FileSystemBlobProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}