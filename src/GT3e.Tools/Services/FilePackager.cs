using System.IO;
using System.IO.Compression;

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
        archive.CreateEntryFromFile(replayFilePath, $"{steamId}.rpy");

        return zipFilePath;
    }
}