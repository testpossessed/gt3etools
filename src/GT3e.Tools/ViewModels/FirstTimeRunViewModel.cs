using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class FirstTimeRunViewModel : ObservableObject
{
    private Visibility broadcastingCheckFailPanelVisibility;
    private Visibility broadcastingCheckSuccessPanelVisibility;
    private bool changeSettingsForMe;
    private Visibility changeSettingsForMeVisibility;
    private Visibility firstTimeRunVisibility;
    private Visibility installationCheckFailPanelVisibility;
    private Visibility installationCheckSuccessPanelVisibility;
    private bool isComplete;
    private bool isFinishEnabled;
    private bool isNextEnabled;
    private Visibility openBroadcastingFileVisibility;
    private int pageIndex;
    private Visibility retryBroadcastingCheckVisibility;
    private string broadcastingPort;

    public FirstTimeRunViewModel()
    {
        this.CancelCommand = new RelayCommand(this.HandleCancelCommand);
        this.FinishCommand = new RelayCommand(this.HandleFinish);
        this.NextCommand = new RelayCommand(this.HandleNextCommand);
        this.RetryBroadcastingCheckCommand = new RelayCommand(this.ProcessBroadcastingSettingsCheck);
        this.OpenBroadcastingFileCommand = new RelayCommand(this.HandleOpenBroadcastingFileCommand);
        this.RetryInstallationCheck = new RelayCommand(this.ProcessInstallationCheck);
        this.InstallationCheckSuccessPanelVisibility = Visibility.Hidden;
        this.InstallationCheckFailPanelVisibility = Visibility.Hidden;
        this.BroadcastingCheckFailPanelVisibility = Visibility.Hidden;
        this.BroadcastingCheckSuccessPanelVisibility = Visibility.Hidden;
        this.OpenBroadcastingFileVisibility = Visibility.Visible;
        this.RetryBroadcastingCheckVisibility = Visibility.Hidden;
        this.IsNextEnabled = true;
        this.IsFinishEnabled = false;
    }

    public ICommand CancelCommand { get; }
    public ICommand FinishCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand OpenBroadcastingFileCommand { get; }
    public ICommand RetryBroadcastingCheckCommand { get; }
    public ICommand RetryInstallationCheck { get; }

    public Visibility BroadcastingCheckFailPanelVisibility
    {
        get => this.broadcastingCheckFailPanelVisibility;
        set => this.SetProperty(ref this.broadcastingCheckFailPanelVisibility, value);
    }

    public Visibility BroadcastingCheckSuccessPanelVisibility
    {
        get => this.broadcastingCheckSuccessPanelVisibility;
        set => this.SetProperty(ref this.broadcastingCheckSuccessPanelVisibility, value);
    }
    public bool ChangeSettingsForMe
    {
        get => this.changeSettingsForMe;
        set
        {
            this.SetProperty(ref this.changeSettingsForMe, value);
            this.OpenBroadcastingFileVisibility = value? Visibility.Hidden: Visibility.Visible;
            this.RetryBroadcastingCheckVisibility = Visibility.Hidden;
            this.IsNextEnabled = value;
        }
    }
    public Visibility ChangeSettingsForMeVisibility
    {
        get => this.changeSettingsForMeVisibility;
        set => this.SetProperty(ref this.changeSettingsForMeVisibility, value);
    }

    public Visibility FirstTimeRunVisibility
    {
        get => this.firstTimeRunVisibility;
        set => this.SetProperty(ref this.firstTimeRunVisibility, value);
    }
    public Visibility InstallationCheckFailPanelVisibility
    {
        get => this.installationCheckFailPanelVisibility;
        set => this.SetProperty(ref this.installationCheckFailPanelVisibility, value);
    }

    public Visibility InstallationCheckSuccessPanelVisibility
    {
        get => this.installationCheckSuccessPanelVisibility;
        set => this.SetProperty(ref this.installationCheckSuccessPanelVisibility, value);
    }
    public bool IsFinishEnabled
    {
        get => this.isFinishEnabled;
        set => this.SetProperty(ref this.isFinishEnabled, value);
    }
    public bool IsNextEnabled
    {
        get => this.isNextEnabled;
        set => this.SetProperty(ref this.isNextEnabled, value);
    }
    public Visibility OpenBroadcastingFileVisibility
    {
        get => this.openBroadcastingFileVisibility;
        set => this.SetProperty(ref this.openBroadcastingFileVisibility, value);
    }
    public Visibility RetryBroadcastingCheckVisibility
    {
        get => this.retryBroadcastingCheckVisibility;
        set => this.SetProperty(ref this.retryBroadcastingCheckVisibility, value);
    }
    public string BroadcastingPort
    {
        get => this.broadcastingPort;
        set => this.SetProperty(ref this.broadcastingPort, value);
    }
    private void HandleCancelCommand()
    {
        if(this.isComplete)
        {
            return;
        }

        var message =
            $"You have not completed initialisation, GT3e Tools will exit if you continue. The wizard will run again the next time you start GT3e Tools.{Environment.NewLine}{Environment.NewLine}Do you wish to continue closing?";
        var messageBoxResult = MessageBox.Show(Application.Current.MainWindow!,
            message,
            "Initialisation Incomplete!!",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if(messageBoxResult == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }

    private void HandleFinish() {
    
        var userSettings = SettingsProvider.GetSettings();
        userSettings.IsInitialised = true;
        SettingsProvider.SaveSettings(userSettings);
        this.FirstTimeRunVisibility = Visibility.Hidden;
    }

    private void HandleNextCommand()
    {
        this.pageIndex++;
        switch(this.pageIndex)
        {
            case 1:
                this.ProcessInstallationCheck();
                break;
            case 2:
                this.ProcessBroadcastingSettingsCheck();
                break;
        }
    }

    private void HandleOpenBroadcastingFileCommand()
    {
        this.OpenBroadcastingFileVisibility = Visibility.Hidden;
        this.RetryBroadcastingCheckVisibility = Visibility.Visible;
        this.ChangeSettingsForMeVisibility = Visibility.Hidden;
        Process.Start("notepad.exe", PathProvider.AccBroadcastingSettingsFilePath);
    }

    private void ProcessBroadcastingSettingsCheck()
    {
        this.IsNextEnabled = false;
        this.IsFinishEnabled = false;
        var settings = AccConfigProvider.GetBroadcastingSettings();
        if(settings?.UpdListenerPort > 0)
        {
            this.BroadcastingCheckSuccessPanelVisibility = Visibility.Visible;
            this.BroadcastingCheckFailPanelVisibility = Visibility.Hidden;
            this.IsFinishEnabled = true;
            this.BroadcastingPort = settings.UpdListenerPort.ToString();
        }
        else
        {
            this.BroadcastingCheckSuccessPanelVisibility = Visibility.Hidden;
            this.BroadcastingCheckFailPanelVisibility = Visibility.Visible;
            this.ChangeSettingsForMeVisibility = Visibility.Visible;
            this.OpenBroadcastingFileVisibility = Visibility.Visible;
            this.RetryBroadcastingCheckVisibility = Visibility.Hidden;
        }
    }

    private void ProcessInstallationCheck()
    {
        this.IsNextEnabled = false;
        if(Directory.Exists(PathProvider.AccConfigFolderPath))
        {
            this.InstallationCheckSuccessPanelVisibility = Visibility.Visible;
            this.InstallationCheckFailPanelVisibility = Visibility.Hidden;
            this.IsNextEnabled = true;
        }
        else
        {
            this.InstallationCheckSuccessPanelVisibility = Visibility.Hidden;
            this.InstallationCheckFailPanelVisibility = Visibility.Visible;
        }
    }
}