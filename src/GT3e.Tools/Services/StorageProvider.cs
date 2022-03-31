using System;
using Azure.Storage.Blobs;

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
            blobClient.Upload(zipFilePath, true);
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