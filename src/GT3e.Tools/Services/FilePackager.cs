using System.IO;
using System.IO.Compression;
using System.Threading;
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

    internal static string PackageCustomSkin(string skinName,
        string customCarFilePath,
        string customSkinFolderPath)
    {
        var zipFilePath = Path.Combine(PathProvider.AppDataFolderPath, $"{skinName}.zip");
        using var zipToOpen = new FileStream(zipFilePath, FileMode.Create);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
        archive.CreateEntryFromFile(customCarFilePath, $"{skinName}.json");
        Thread.Sleep(1000);
        archive.CreateEntryFromFile(customSkinFolderPath, $"{skinName}.rpy");

        return zipFilePath;
    }
}