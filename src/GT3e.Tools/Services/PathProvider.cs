using System;
using System.Diagnostics;
using System.IO;

namespace GT3e.Tools.Services;

public static class PathProvider
{
    private const string AccFolderName = "Assetto Corsa Competizione";
    private const string AccSetupsFolderName = "Setups";
    private const string AccConfigFolderName = "Config";
    public const string AppSettingsFileName = "appsettings.json";
    private const string UserSettingsFileName = "userSettings.json";
    private const string BroadcastingSettingsFileName = "broadcasting.json";

    static PathProvider()
    {
        var processName = Process.GetCurrentProcess()
                                 .ProcessName;
        var localAppDataFolderPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        AppDataFolderPath = Path.Combine(localAppDataFolderPath, processName);
        AppSettingsFilePath = Path.Combine(AppDataFolderPath, AppSettingsFileName);
        var executionFolder = AppDomain.CurrentDomain.BaseDirectory;
        AppFolderPath = Path.GetDirectoryName(executionFolder);
        DefaultSettingsFilePath = Path.Combine(AppFolderPath!, AppSettingsFileName);
        var myDocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        AccSetupsFolderPath = Path.Combine(myDocumentsFolderPath, AccFolderName, AccSetupsFolderName);
        UserSettingsFilePath = Path.Combine(AppDataFolderPath, UserSettingsFileName);
        AccConfigFolderPath = Path.Combine(myDocumentsFolderPath, AccFolderName, AccConfigFolderName);
        AccBroadcastingSettingsFilePath = Path.Combine(AccConfigFolderPath, BroadcastingSettingsFileName);
    }

    public static string AccConfigFolderPath { get; }
    public static string AccSetupsFolderPath { get; }
    public static string AppDataFolderPath { get; }
    public static string AppFolderPath { get; }
    public static string AppSettingsFilePath { get; }
    public static string AccBroadcastingSettingsFilePath { get; }
    public static string DefaultSettingsFilePath { get; }
    public static string UserSettingsFilePath { get; }

    public static string GetLastFolderName(string path)
    {
        var folderPath = Path.HasExtension(path)? Path.GetDirectoryName(path): path;
        return new DirectoryInfo(folderPath!).Name;
    }
}