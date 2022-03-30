using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace GT3e.Tools.Services;

internal class StorageProvider
{
    internal static async Task UploadVerificationFiles(string steamId,
        string resultsFilePath,
        string replayFilePath)
    {
        var systemSettings = SettingsProvider.GetSystemSettings();
        var blobContainerClient =
            new BlobContainerClient(systemSettings.StorageConnectionString, "verification-tests");
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient($"{steamId}-results.json");
        await blobClient.UploadAsync(resultsFilePath);

        var replayBlobClient = blobContainerClient.GetBlobClient($"{steamId}-replay-rpy");
        await replayBlobClient.UploadAsync(replayFilePath);
    }
}