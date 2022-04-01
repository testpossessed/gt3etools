using System;
using System.IO;
using System.Threading;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace GT3e.Tools.Services;

internal class StorageProvider
{
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

                                                   var progress = Math.Floor((double)bytesUploaded / totalBytes * 100);
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
}