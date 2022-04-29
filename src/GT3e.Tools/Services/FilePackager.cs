using System.IO;
using System.IO.Compression;
using System.Threading;
using Acc.Lib;
using Acc.Lib.Models.Customs;

namespace GT3e.Tools.Services;

internal class FilePackager
{
  internal static string PackageCustomSkin(CustomSkin customSkin)
  {
    var zipFilePath = Path.Combine(PathProvider.AppDataFolderPath, $"{customSkin.Name}.zip");
    var imageFilePaths = Directory.GetFiles(customSkin.FolderPath, "*.png");

    using var zipToOpen = new FileStream(zipFilePath, FileMode.Create);
    using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

    foreach(var imageFilePath in imageFilePaths)
    {
      archive.CreateEntryFromFile(imageFilePath,
        $"Customs/Liveries/{customSkin.Name}/{Path.GetFileName(imageFilePath)}");
    }

    return zipFilePath;
  }

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

  internal static void UnpackCustomSkin(string zipFilePath)
  {
    using var zipToOpen = new FileStream(zipFilePath, FileMode.Open);
    using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
    archive.ExtractToDirectory(AccPathProvider.DocumentsFolderPath, true);
  }
}