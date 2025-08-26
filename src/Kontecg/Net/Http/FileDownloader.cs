using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Polly;
using Polly.Retry;
using Velopack.Sources;

namespace Kontecg.Net.Http
{
    internal class FileDownLoader : IFileDownloader
    {
        private readonly IPreconfiguredHttpClientProvider _preconfiguredHttpClientProvider;

        public FileDownLoader(IPreconfiguredHttpClientProvider preconfiguredHttpClientProvider)
        {
            _preconfiguredHttpClientProvider = preconfiguredHttpClientProvider;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public async Task DownloadFile(string url, string targetFile, Action<int> progress, IDictionary<string, string> headers = null, double timeout = 30,
            CancellationToken cancelToken = new CancellationToken())
        {
            try
            {
                AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
                    .Handle<HttpRequestException>(exception =>
                    {
                        Logger.Error(
                            $"Can't download process request due an error. Exception: {exception}");
                        return true;
                    })
                    .Or<TaskCanceledException>()
                    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(5, retryCount => TimeSpan.FromMilliseconds(1000),
                        (result, timeSpan, retryCount, context) =>
                        {
                            Logger.Warn(
                                $"Downloader service delivery attempt {retryCount} failed, next attempt in {timeSpan.TotalMilliseconds} ms. Result: {result.Result?.StatusCode}. ");
                        });

                using HttpClient httpClient = _preconfiguredHttpClientProvider.HttpClient;
                httpClient.Timeout = TimeSpan.FromSeconds(timeout);

                if (headers != null)
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        if (!httpClient.DefaultRequestHeaders.Contains(header.Key)) ;
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                HttpResponseMessage response = await retryPolicy.ExecuteAsync(() => httpClient.GetAsync(url, cancelToken));
                if (response.IsSuccessStatusCode)
                {
                    long? totalBytes = response.Content.Headers.ContentLength;
                    Stream stream = await response.Content.ReadAsStreamAsync(cancelToken);
                    await using FileStream fileStream = File.Create(targetFile);
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    long totalRead = 0;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancelToken)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancelToken);
                        totalRead += bytesRead;
                        double progressPercentage =
                            totalBytes.HasValue ? (double)totalRead / totalBytes.Value * 100 : 0;
                        progress?.Invoke((int)Math.Round(progressPercentage));
                    }
                }
                else
                {
                    Logger.Warn($"HTTP {response.StatusCode}. {response.ReasonPhrase}.");
                }
            }
            catch (InvalidOperationException ex)
            {
                Logger.Warn($"Bad url format on '{url}'", ex);
            }
        }

        /// <inheritdoc />
        public async Task<byte[]> DownloadBytes(string url, IDictionary<string, string> headers = null, double timeout = 30)
        {
            try
            {
                using HttpClient httpClient = _preconfiguredHttpClientProvider.HttpClient;
                httpClient.Timeout = TimeSpan.FromSeconds(timeout);

                if (headers != null)
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        if (!httpClient.DefaultRequestHeaders.Contains(header.Key)) ;
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                return await httpClient.GetByteArrayAsync(url.ToLower());
            }
            catch (InvalidOperationException ex)
            {
                Logger.Warn($"Bad url format on '{url}'", ex);
            }
            catch (HttpRequestException ex)
            {
                //TODO: Retry policy implement here
                Logger.Warn("Implement retry policy", ex);
            }
            catch (TaskCanceledException ex)
            {
                Logger.Warn("Operation was cancelled due a timeout. Can't retry here!", ex);
            }

            return [];
        }

        /// <inheritdoc />
        public async Task<string> DownloadString(string url, IDictionary<string, string> headers = null, double timeout = 30)
        {
            try
            {
                using HttpClient httpClient = _preconfiguredHttpClientProvider.HttpClient;
                httpClient.Timeout = TimeSpan.FromSeconds(timeout);

                if (headers != null)
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        if(!httpClient.DefaultRequestHeaders.Contains(header.Key));
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                return await httpClient.GetStringAsync(url.ToLower());
            }
            catch (InvalidOperationException ex)
            {
                Logger.Warn($"Bad url format on '{url}'", ex);
            }
            catch (HttpRequestException ex)
            {
                //TODO: Retry policy implement here
                Logger.Warn("Implement retry policy", ex);
            }
            catch (TaskCanceledException ex)
            {
                Logger.Warn("Operation was cancelled due a timeout. Can't retry here!", ex);
            }

            return string.Empty;
        }
    }
}
