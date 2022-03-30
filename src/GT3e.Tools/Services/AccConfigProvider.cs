using System;
using System.IO;
using System.Text;
using GT3e.Tools.Acc.Models.Config;
using GT3e.Tools.Acc.Models.Config.SeasonEntity;
using Newtonsoft.Json;

namespace GT3e.Tools.Services;

internal class AccConfigProvider
{
    internal static BroadcastingSettings? GetBroadcastingSettings()
    {
        if(!File.Exists(PathProvider.AccBroadcastingSettingsFilePath))
        {
            return null;
        }

        var content = File.ReadAllText(PathProvider.AccBroadcastingSettingsFilePath, Encoding.UTF8);
        content = content.Replace(Environment.NewLine, "")
                         .Replace("\0", "")
                         .Replace("\n", "");
        return JsonConvert.DeserializeObject<BroadcastingSettings>(content);
    }

    internal static SeasonSettings? GetSeasonSettings()
    {
        if(!File.Exists(PathProvider.AccSeasonSettingsFilePath))
        {
            return null;
        }

        var content = File.ReadAllText(PathProvider.AccSeasonSettingsFilePath, Encoding.UTF8);
        content = content.Replace(Environment.NewLine, "")
                         .Replace("\0", "")
                         .Replace("\n", "");
        return JsonConvert.DeserializeObject<SeasonSettings>(content);
    }

    internal static void SaveBroadcastingSettings(BroadcastingSettings settings)
    {
        var json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(PathProvider.AccBroadcastingSettingsFilePath, json);
    }
}