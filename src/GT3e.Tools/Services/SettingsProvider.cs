using System.IO;
using System.Linq;
using System.Reflection;
using GT3e.Tools.Models;
using Newtonsoft.Json;

namespace GT3e.Tools.Services
{
  internal static class SettingsProvider
  {
    internal static UserSettings GetUserSettings()
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

    internal static void SaveUserSettings(UserSettings settings)
    {
      var json = JsonConvert.SerializeObject(settings);
      File.WriteAllText(PathProvider.UserSettingsFilePath, json);
    }

    internal static SystemSettings GetSystemSettings()
    {
        const string resourcePath = "GT3e.Tools.SystemSettings.json";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourcePath);
        using var reader = new StreamReader(stream!);
        var content = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<SystemSettings>(content)!;

    }
  }
}