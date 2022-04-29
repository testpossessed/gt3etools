using System;
using System.IO;
using Acc.Lib;

namespace GT3e.Tools.Services;

internal class VerificationTestRunner
{
  private Action<bool> callback;
  private FileSystemWatcher fileSystemWatcher;
  private string replayFilePath;
  private string resultFilePath;
  private string steamId;

  internal void Cancel()
  {
    const string message = "Cancelling Verification Test";
    LogWriter.Info(message);
    ConsoleLog.Write($"{message}...");
    if(this.fileSystemWatcher != null)
    {
      this.fileSystemWatcher.EnableRaisingEvents = false;
      this.fileSystemWatcher.Dispose();
      this.fileSystemWatcher = null;
    }

    ConsoleLog.Write("Finished");
    this.callback(false);
  }

  internal void Run(string steamId, Action<bool> callback)
  {
    this.steamId = steamId;
    this.callback = callback;
    this.WaitForResultsFile();
  }

  private void HandleReplayFileCreated(object sender, FileSystemEventArgs eventArgs)
  {
    this.replayFilePath = eventArgs.FullPath;
    var message = $"Replay file detected: {this.replayFilePath}";
    LogWriter.Info(message);
    ConsoleLog.Write(message);

    this.fileSystemWatcher.EnableRaisingEvents = false;
    this.fileSystemWatcher.Created -= this.HandleReplayFileCreated;
    this.fileSystemWatcher.Dispose();
    this.fileSystemWatcher = null!;

    if(!StorageProvider.UploadVerificationFiles(this.steamId,
         this.resultFilePath,
         this.replayFilePath))
    {
      this.callback(false);
      return;
    }

    this.UpdateUserSettings();
    this.callback(true);
  }

  private void HandleResultsFileDetected(object sender, FileSystemEventArgs eventArgs)
  {
    this.resultFilePath = eventArgs.FullPath;
    var message = $"Results file detected: {this.resultFilePath}";
    LogWriter.Info(message);
    ConsoleLog.Write(message);

    this.fileSystemWatcher.EnableRaisingEvents = false;
    this.fileSystemWatcher.Created -= this.HandleResultsFileDetected;
    this.fileSystemWatcher.Dispose();
    this.fileSystemWatcher = null!;

    // if(!this.ValidateSessionSettings())
    // {
    //     this.callback(false);
    //     return;
    // }

    this.WaitForReplayFile();
  }

  private void UpdateUserSettings()
  {
    var userSettings = SettingsProvider.GetUserSettings();
    userSettings.IsVerificationPending = true;
    userSettings.SteamId = this.steamId;
    SettingsProvider.SaveUserSettings(userSettings);
  }

  private bool ValidateSessionSettings()
  {
    var settings = AccConfigProvider.GetSeasonSettings();
    LogWriter.Info("Verifying settings");
    ConsoleLog.Write("Verifying settings...");

    if(settings!.SessionGameplay.AggroMultiplier >= 90
       && settings.SessionGameplay.SkillMultiplier >= 90 && settings.Events[0]
         .TrackName.ToLowerInvariant() == "zolder")
    {
      return true;
    }

    LogWriter.Info("Invalid session settings");
    ConsoleLog.Write(
      "The gameplay settings are not valid for the test session. Please make sure you have configured the session and track correctly and try again.");
    return false;
  }

  private void WaitForReplayFile()
  {
    LogWriter.Info("Waiting for replay file");
    ConsoleLog.Write("Waiting for replay file...");
    this.fileSystemWatcher = new FileSystemWatcher(AccPathProvider.SavedReplaysFolderPath, "*.rpy");
    this.fileSystemWatcher.Created += this.HandleReplayFileCreated;
    this.fileSystemWatcher.EnableRaisingEvents = true;
  }

  private void WaitForResultsFile()
  {
    LogWriter.Info("Waiting for session results file");
    ConsoleLog.Write("Waiting for session results file...");
    this.fileSystemWatcher = new FileSystemWatcher(AccPathProvider.ResultFolderPath, "Race.json");
    this.fileSystemWatcher.Changed += this.HandleResultsFileDetected;
    this.fileSystemWatcher.Created += this.HandleResultsFileDetected;
    this.fileSystemWatcher.EnableRaisingEvents = true;
  }
}