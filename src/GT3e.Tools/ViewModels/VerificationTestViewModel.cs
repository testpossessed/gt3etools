using System;
using System.IO;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using GT3e.Tools.Acc;
using GT3e.Tools.Acc.Messages;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class VerificationTestViewModel : ObservableObject
{
    private FileSystemWatcher fileSystemWatcher = null!;
    private bool isCancelEnabled;
    private bool isStartEnabled;
    private string replayFilePath;
    private string resultFilePath;
    private string steamId;
    private Visibility verificationTestVisibility;

    public VerificationTestViewModel()
    {
        this.StartCommand = new RelayCommand(this.HandleStartCommand);
        this.CancelCommand = new RelayCommand(this.HandleCancelCommand);
        this.IsStartEnabled = false;
        this.IsCancelEnabled = false;

        var userSettings = SettingsProvider.GetUserSettings();
        this.SteamId = userSettings.SteamId;
    }

    public ICommand CancelCommand { get; }

    public ICommand StartCommand { get; }
    public bool IsCancelEnabled
    {
        get => this.isCancelEnabled;
        set => this.SetProperty(ref this.isCancelEnabled, value);
    }

    public bool IsStartEnabled
    {
        get => this.isStartEnabled;
        set => this.SetProperty(ref this.isStartEnabled, value);
    }
    public string SteamId
    {
        get => this.steamId;
        set
        {
            this.IsStartEnabled = !string.IsNullOrWhiteSpace(value);
            this.SetProperty(ref this.steamId, value);
        }
    }

    public Visibility VerificationTestVisibility
    {
        get => this.verificationTestVisibility;
        set => this.SetProperty(ref this.verificationTestVisibility, value);
    }

    private void HandleCancelCommand()
    {
        var message = "Cancelling Verification Test";
        LogWriter.Info(message);
        ConsoleLog.Write($"{message}...");

        this.fileSystemWatcher.EnableRaisingEvents = false;
        this.fileSystemWatcher.Dispose();
        this.fileSystemWatcher = null;
        ConsoleLog.Write("Finished");
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

        if(!StorageProvider.UploadVerificationFiles(this.SteamId, this.resultFilePath, this.replayFilePath))
        {
            return;
        }

        this.UpdateUserSettings();

        this.VerificationTestVisibility = Visibility.Hidden;
    }

    private void UpdateUserSettings()
    {
        var userSettings = SettingsProvider.GetUserSettings();
        userSettings.IsVerificationPending = true;
        userSettings.SteamId = this.SteamId;
        SettingsProvider.SaveSettings(userSettings);
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

        if(!this.ValidateSessionSettings())
        {
            this.IsStartEnabled = true;
            this.IsCancelEnabled = false;
            return;
        }

        this.WaitForReplayFile();
    }

    private void HandleStartCommand()
    {
        this.IsStartEnabled = false;
        this.IsCancelEnabled = true;
        this.WaitForResultsFile();
    }

    private bool ValidateSessionSettings()
    {
        var settings = AccConfigProvider.GetSeasonSettings();
        LogWriter.Info("Verifying settings");
        ConsoleLog.Write("Verifying settings...");

        if(settings!.SessionGameplay.AggroMultiplier >= 90 && settings.SessionGameplay.SkillMultiplier >= 90 && settings.Events[0].TrackName.ToLowerInvariant() == "zolder")
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
        this.fileSystemWatcher = new FileSystemWatcher(PathProvider.AccSavedReplaysFolderPath, "*.rpy");
        this.fileSystemWatcher.Changed += this.HandleReplayFileCreated;
        this.fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void WaitForResultsFile()
    {
        LogWriter.Info("Waiting for session results file");
        ConsoleLog.Write("Waiting for session results file...");
        this.fileSystemWatcher = new FileSystemWatcher(PathProvider.AccResultFolderPath, "Race.json");
        this.fileSystemWatcher.Changed += this.HandleResultsFileDetected;
        this.fileSystemWatcher.Created += this.HandleResultsFileDetected;
        this.fileSystemWatcher.EnableRaisingEvents = true;
    }
}