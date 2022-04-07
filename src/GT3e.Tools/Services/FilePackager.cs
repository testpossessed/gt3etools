using System.IO;
using System.IO.Compression;
using System.Threading;
using GT3e.Acc.Models.Customs;
using GT3e.Tools.Models;

namespace GT3e.Tools.Services;

internal class FilePackager
{
    internal static string PackageVerificationTestFiles(string steamId,
        string resultFilePath,
        string replayFilePath)
    {
        var zipFilePath = Path.Combine(PathProvider.AppDataFolderPath, $"{steamId}.zip");
        using var zipToOpen = new FileStream(zipFilePath, FileMode.Create);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
        archive.CreateEntryFromFile(resultFilePath, $"{steamId}.json");
        Thread.Sleep(1000);
        archive.CreateEntryFromFile(replayFilePath, $"{steamId}.rpy");

        return zipFilePath;
    }

    internal static string PackageCustomSkin(CustomSkin customSkin)
    {
        var zipFilePath = Path.Combine(PathProvider.AppDataFolderPath, $"{customSkin.Name}.zip");
        var imageFilePaths = Directory.GetFiles(customSkin.FolderPath, "*.png");

        using var zipToOpen = new FileStream(zipFilePath, FileMode.Create);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

        foreach(var imageFilePath in imageFilePaths)
        {
            archive.CreateEntryFromFile(imageFilePath, $"Customs/Liveries/{customSkin.Name}/{Path.GetFileName(imageFilePath)}");
        }
        
        return zipFilePath;
    }
}