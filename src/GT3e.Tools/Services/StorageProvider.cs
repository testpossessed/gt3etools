using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GT3e.Acc.Models.Customs;
using GT3e.Tools.Models;
using Newtonsoft.Json;

namespace GT3e.Tools.Services;

internal class StorageProvider
{
    public static async Task UploadCustomSkin(CustomSkin customSkin)
    {
        var systemSettings = SettingsProvider.GetSystemSettings();
        var zipFilePath = FilePackager.PackageCustomSkin(customSkin);

        var message = $"Created {zipFilePath}";
        LogWriter.Info(message);
        ConsoleLog.Write(message);
        var blobContainerClient =
            new BlobContainerClient(systemSettings.StorageConnectionString, "custom-skins");
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient($"{customSkin.Name}.zip");
        var fileInfo = new FileInfo(zipFilePath);
        var totalBytes = fileInfo.Length;
        var progressHandler = new Progress<long>();
        var lastProgress = 0D;
        progressHandler.ProgressChanged += (sender, bytesUploaded) =>
                                           {
                                               if(bytesUploaded <= 0)
                                               {
                                                   return;
                                               }

                                               var progress =
                                                   Math.Floor((double) bytesUploaded / totalBytes * 100);
                                               if(progress > lastProgress && progress % 10 == 0)
                                               {
                                                   ConsoleLog.Write($"Uploaded {progress}%...");
                                               }

                                               lastProgress = progress;
                                           };

        var options = new BlobUploadOptions
        {
            ProgressHandler = progressHandler,
            TransferOptions = new StorageTransferOptions
            {
                MaximumTransferSize = 4 * 1024 * 1024,
                InitialTransferSize = 4 * 1024 * 1024
            }
        };

        await blobClient.UploadAsync(zipFilePath, options, CancellationToken.None);
        message = $"Uploaded {zipFilePath}";
        LogWriter.Info(message);
        ConsoleLog.Write(message);
    }

    internal static DriverStats? GetDriverStats(string steamId)
    {
        var message = "Checking for driver stats";
        LogWriter.Info(message);
        ConsoleLog.Write($"{message}...");

        var systemSettings = SettingsProvider.GetSystemSettings();

        var blobContainerClient =
            new BlobContainerClient(systemSettings.StorageConnectionString, "driver-stats");
        blobContainerClient.CreateIfNotExists();

        var blobClient = blobContainerClient.GetBlobClient($"{steamId}.json");
        var exists = blobClient.Exists();
        if(!exists.Value)
        {
            message = "Stats not found, verification test review is still pending";
            LogWriter.Info(message);
            ConsoleLog.Write(message);
            return null;
        }

        var progressHandler = new Progress<long>();
        var lastProgress = 0D;
        var totalBytes = blobClient.GetProperties()
                                   .Value.ContentLength;
        progressHandler.ProgressChanged += (sender, bytesUploaded) =>
                                           {
                                               if(bytesUploaded <= 0)
                                               {
                                                   return;
                                               }

                                               var progress =
                                                   Math.Floor((double) bytesUploaded / totalBytes * 100);
                                               if(progress > lastProgress && progress % 10 == 0)
                                               {
                                                   ConsoleLog.Write($"Downloaded {progress}%...");
                                               }

                                               lastProgress = progress;
                                           };

        var statsBlob = blobClient.DownloadContent(progressHandler: progressHandler);
        var jsonBytes = statsBlob.Value.Content.ToArray();
        var json = Encoding.UTF8.GetString(jsonBytes);
        var driverStats = JsonConvert.DeserializeObject<DriverStats>(json);

        message = "Downloaded stats";
        LogWriter.Info(message);
        ConsoleLog.Write(message);

        return driverStats;
    }

    internal static bool UploadVerificationFiles(string steamId,
        string resultsFilePath,
        string replayFilePath)
    {
        try
        {
            var systemSettings = SettingsProvider.GetSystemSettings();

            var zipFilePath =
                FilePackager.PackageVerificationTestFiles(steamId, resultsFilePath, replayFilePath);

            var message = $"Created {zipFilePath}";
            LogWriter.Info(message);
            ConsoleLog.Write(message);
            var blobContainerClient =
                new BlobContainerClient(systemSettings.StorageConnectionString, "verification-tests");
            blobContainerClient.CreateIfNotExists();

            var blobClient = blobContainerClient.GetBlobClient($"{steamId}.zip");
            blobClient.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

            var fileInfo = new FileInfo(zipFilePath);
            var totalBytes = fileInfo.Length;
            var progressHandler = new Progress<long>();
            var lastProgress = 0D;
            progressHandler.ProgressChanged += (sender, bytesUploaded) =>
                                               {
                                                   if(bytesUploaded <= 0)
                                                   {
                                                       return;
                                                   }

                                                   var progress =
                                                       Math.Floor((double) bytesUploaded / totalBytes * 100);
                                                   if(progress > lastProgress && progress % 10 == 0)
                                                   {
                                                       ConsoleLog.Write($"Uploaded {progress}%...");
                                                   }

                                                   lastProgress = progress;
                                               };

            var options = new BlobUploadOptions
            {
                ProgressHandler = progressHandler,
                TransferOptions = new StorageTransferOptions
                {
                    MaximumTransferSize = 4 * 1024 * 1024,
                    InitialTransferSize = 4 * 1024 * 1024
                }
            };
            blobClient.Upload(zipFilePath, options, CancellationToken.None);
            message = $"Uploaded {zipFilePath}";
            LogWriter.Info(message);
            ConsoleLog.Write(message);
            return true;
        }
        catch(Exception exception)
        {
            LogWriter.Error(exception, "Unexpected error uploading verification test files");
            ConsoleLog.Write($"Unexpected error uploading verification test files: {exception.Message}");
            return false;
        }
    }

    
    public static Task<IEnumerable<CustomSkinInfo>> GetCustomSkins()
    {
        var systemSettings = SettingsProvider.GetSystemSettings();

        var blobContainerClient =
            new BlobContainerClient(systemSettings.StorageConnectionString, "custom-skins");
        var pages = blobContainerClient.GetBlobs()
                                       .AsPages();

        return Task.FromResult<IEnumerable<CustomSkinInfo>>(
            (from page in pages from item in page.Values select new CustomSkinInfo
                {
                    FileName = item.Name,
                    LastModifiedUtc = item.Properties.LastModified.GetValueOrDefault().ToUniversalTime().DateTime,
                    ContentLength = item.Properties.ContentLength.GetValueOrDefault()
                }).ToList());
    }

    public static async Task<string> DownloadCustomSkin(CustomSkinInfo customSkin)
    {
        var message = $"Downloading skin {customSkin.Name}";
        LogWriter.Info(message);
        ConsoleLog.Write(message);

        var systemSettings = SettingsProvider.GetSystemSettings();

        var blobContainerClient =
            new BlobContainerClient(systemSettings.StorageConnectionString, "custom-skins");
        var blobClient = blobContainerClient.GetBlobClient(customSkin.FileName);

        var progressHandler = new Progress<long>();
        var lastProgress = 0D;
        progressHandler.ProgressChanged += (sender, bytesDownloaded) =>
                                           {
                                               if(bytesDownloaded <= 0)
                                               {
                                                   return;
                                               }

                                               var progress = customSkin.ContentLength > 0 ? Math.Floor((double) bytesDownloaded / customSkin.ContentLength * 100): bytesDownloaded;
                                               if(progress > lastProgress && progress % 10 == 0)
                                               {
                                                   ConsoleLog.Write($"Downloaded {progress}{(customSkin.ContentLength > 0? "%":" bytes")}...");
                                               }

                                               lastProgress = progress;
                                           };

        var options = new BlobDownloadToOptions
        {
            ProgressHandler = progressHandler,
            TransferOptions = new StorageTransferOptions
            {
                MaximumTransferSize = 4 * 1024 * 1024,
                InitialTransferSize = 4 * 1024 * 1024
            }
        };

        var zipFilePath = Path.Combine(PathProvider.SyncDownloadsFolderPath, customSkin.FileName);
        await blobClient.DownloadToAsync(zipFilePath, options, CancellationToken.None);
        message = $"Downloaded skin {customSkin.Name}";
        LogWriter.Info(message);
        ConsoleLog.Write(message);

        return zipFilePath;
    }
}