namespace GT3e.Tools.Models;

internal class PackageInfo
{
    public PackageInfo(string filePath, long size)
    {
        this.FilePath = filePath;
        this.Size = size;
    }

    public string FilePath { get; }
    public long Size { get; }
}