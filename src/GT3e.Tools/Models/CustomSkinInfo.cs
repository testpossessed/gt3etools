using System;

namespace GT3e.Tools.Models;

internal class CustomSkinInfo
{
    public string Name => this.FileName.Replace(".zip", "");
    public string FileName { get; set; }
    public DateTime? LastModifiedUtc { get; set; }
}