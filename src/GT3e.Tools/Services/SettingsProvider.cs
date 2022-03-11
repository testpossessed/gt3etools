using System.IO;
using GT3e.Tools.Models;
using Newtonsoft.Json;

namespace GT3e.Tools.Services
{
  internal static class SettingsProvider
  {
    internal static UserSettings GetSettings()
    {
      if(!File.Exists(PathProvider.UserSettingsFilePath))
      {
        return new UserSettings
               {
                 Theme = "Blend",
                 IsInitialised = false
               };
      }

      var json = File.ReadAllText(PathProvider.UserSettingsFilePath);
      return JsonConvert.DeserializeObject<UserSettings>(json);
    }

    internal static void SaveSettings(UserSettings settings)
    {
      var json = JsonConvert.SerializeObject(settings);
      File.WriteAllText(PathProvider.UserSettingsFilePath, json);
    }
  }
}