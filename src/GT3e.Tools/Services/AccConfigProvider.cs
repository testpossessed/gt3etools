using System.IO;
using GT3e.Tools.Models;
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

        var content = File.ReadAllText(PathProvider.AccBroadcastingSettingsFilePath);
        return JsonConvert.DeserializeObject<BroadcastingSettings>(content);
    }


    internal static void SaveBroadcastingSettings(BroadcastingSettings settings)
    {
        var json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(PathProvider.AccBroadcastingSettingsFilePath, json);
    }
}