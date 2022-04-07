using System;
using System.Diagnostics;
using System.IO;

namespace GT3e.Tools.Services;

public static class PathProvider
{
    public const string AppSettingsFileName = "appsettings.json";
    private const string UserSettingsFileName = "userSettings.json";

    static PathProvider()
    {
        var processName = Process.GetCurrentProcess()
                                 .ProcessName;
        var localAppDataFolderPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        AppDataFolderPath = Path.Combine(localAppDataFolderPath, processName);
        AppSettingsFilePath = Path.Combine(AppDataFolderPath, AppSettingsFileName);
        var executionFolder = AppDomain.CurrentDomain.BaseDirectory;
        AppFolderPath = Path.GetDirectoryName(executionFolder)!;
        DefaultSettingsFilePath = Path.Combine(AppFolderPath!, AppSettingsFileName);
        UserSettingsFilePath = Path.Combine(AppDataFolderPath, UserSettingsFileName);
    }

    public static string AppDataFolderPath { get; }
    public static string AppFolderPath { get; }
    public static string AppSettingsFilePath { get; }
    public static string DefaultSettingsFilePath { get; }
    public static string UserSettingsFilePath { get; }
}