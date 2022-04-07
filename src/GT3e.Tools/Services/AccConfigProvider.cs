using System;
using System.IO;
using System.Text;
using GT3e.Acc;
using GT3e.Acc.Models.Config;
using GT3e.Acc.Models.Config.SeasonEntity;
using Newtonsoft.Json;

namespace GT3e.Tools.Services;

internal class AccConfigProvider
{
    internal static BroadcastingSettings? GetBroadcastingSettings()
    {
        if(!File.Exists(AccPathProvider.BroadcastingSettingsFilePath))
        {
            return null;
        }

        var content = File.ReadAllText(AccPathProvider.BroadcastingSettingsFilePath, Encoding.UTF8);
        content = content.Replace(Environment.NewLine, "")
                         .Replace("\0", "")
                         .Replace("\n", "");
        return JsonConvert.DeserializeObject<BroadcastingSettings>(content);
    }

    internal static SeasonSettings? GetSeasonSettings()
    {
        if(!File.Exists(AccPathProvider.SeasonSettingsFilePath))
        {
            return null;
        }

        var content = File.ReadAllText(AccPathProvider.SeasonSettingsFilePath, Encoding.UTF8);
        content = content.Replace(Environment.NewLine, "")
                         .Replace("\0", "")
                         .Replace("\n", "");
        return JsonConvert.DeserializeObject<SeasonSettings>(content);
    }

    internal static void SaveBroadcastingSettings(BroadcastingSettings settings)
    {
        var json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(AccPathProvider.BroadcastingSettingsFilePath, json);
    }
}