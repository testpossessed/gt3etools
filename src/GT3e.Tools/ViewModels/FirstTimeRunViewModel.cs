using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Acc.Lib;
using Acc.Lib.Models.Config;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class FirstTimeRunViewModel : ObservableObject
{
  private Visibility broadcastingCheckFailPanelVisibility;
  private Visibility broadcastingCheckSuccessPanelVisibility;
  private string broadcastingPort;
  private Visibility broadcastingSettingsVisibility;
  private bool changeSettingsForMe;
  private Visibility changeSettingsForMeVisibility;
  private string commandPassword;
  private string connectionPassword;
  private Visibility finishedSummaryVisibility;
  private Visibility firstTimeRunVisibility;
  private Visibility installationCheckFailPanelVisibility;
  private Visibility installationCheckSuccessPanelVisibility;
  private bool isComplete;
  private bool isFinishEnabled;
  private bool isNextEnabled;
  private int listenerPort;
  private Visibility openBroadcastingFileVisibility;
  private int pageIndex;
  private Visibility retryBroadcastingCheckVisibility;

  public FirstTimeRunViewModel()
  {
    this.CancelCommand = new RelayCommand(this.HandleCancelCommand);
    this.FinishCommand = new RelayCommand(this.HandleFinish);
    this.NextCommand = new RelayCommand(this.HandleNextCommand);
    this.RetryBroadcastingCheckCommand = new RelayCommand(this.ProcessBroadcastingSettingsCheck);
    this.OpenBroadcastingFileCommand = new RelayCommand(this.HandleOpenBroadcastingFileCommand);
    this.RetryInstallationCheck = new RelayCommand(this.ProcessInstallationCheck);
    this.SaveBroadcastingSettingsCommand =
      new RelayCommand(this.HandleSaveBroadcastingSettingsCommand);
    this.InstallationCheckSuccessPanelVisibility = Visibility.Collapsed;
    this.InstallationCheckFailPanelVisibility = Visibility.Collapsed;
    this.BroadcastingCheckFailPanelVisibility = Visibility.Collapsed;
    this.BroadcastingCheckSuccessPanelVisibility = Visibility.Collapsed;
    this.OpenBroadcastingFileVisibility = Visibility.Visible;
    this.RetryBroadcastingCheckVisibility = Visibility.Collapsed;
    this.FinishedSummaryVisibility = Visibility.Collapsed;
    this.BroadcastingSettingsVisibility = Visibility.Collapsed;
    this.IsNextEnabled = true;
    this.IsFinishEnabled = false;
    this.ListenerPort = 9000;
    this.CommandPassword = string.Empty;
    this.ConnectionPassword = string.Empty;
  }

  public ICommand CancelCommand { get; }
  public ICommand FinishCommand { get; }
  public ICommand NextCommand { get; }
  public ICommand OpenBroadcastingFileCommand { get; }
  public ICommand RetryBroadcastingCheckCommand { get; }
  public ICommand RetryInstallationCheck { get; }
  public ICommand SaveBroadcastingSettingsCommand { get; }

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
  public string BroadcastingPort
  {
    get => this.broadcastingPort;
    set => this.SetProperty(ref this.broadcastingPort, value);
  }
  public Visibility BroadcastingSettingsVisibility
  {
    get => this.broadcastingSettingsVisibility;
    set => this.SetProperty(ref this.broadcastingSettingsVisibility, value);
  }
  public bool ChangeSettingsForMe
  {
    get => this.changeSettingsForMe;
    set
    {
      this.SetProperty(ref this.changeSettingsForMe, value);
      this.OpenBroadcastingFileVisibility = value? Visibility.Collapsed: Visibility.Visible;
      this.RetryBroadcastingCheckVisibility = Visibility.Collapsed;
      this.IsNextEnabled = value;
    }
  }
  public Visibility ChangeSettingsForMeVisibility
  {
    get => this.changeSettingsForMeVisibility;
    set => this.SetProperty(ref this.changeSettingsForMeVisibility, value);
  }
  public string CommandPassword
  {
    get => this.commandPassword;
    set => this.SetProperty(ref this.commandPassword, value);
  }
  public string ConnectionPassword
  {
    get => this.connectionPassword;
    set => this.SetProperty(ref this.connectionPassword, value);
  }
  public Visibility FinishedSummaryVisibility
  {
    get => this.finishedSummaryVisibility;
    set => this.SetProperty(ref this.finishedSummaryVisibility, value);
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
  public int ListenerPort
  {
    get => this.listenerPort;
    set => this.SetProperty(ref this.listenerPort, value);
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

  private void HandleFinish()
  {
    var userSettings = SettingsProvider.GetUserSettings();
    userSettings.IsInitialised = true;
    SettingsProvider.SaveUserSettings(userSettings);
    this.FirstTimeRunVisibility = Visibility.Collapsed;
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
      case 3:
        this.ProcessSummaryOrSaveSettings();
        break;
    }
  }

  private void HandleOpenBroadcastingFileCommand()
  {
    this.OpenBroadcastingFileVisibility = Visibility.Collapsed;
    this.RetryBroadcastingCheckVisibility = Visibility.Visible;
    this.ChangeSettingsForMeVisibility = Visibility.Collapsed;
    Process.Start("notepad.exe", AccPathProvider.BroadcastingSettingsFilePath);
  }

  private void HandleSaveBroadcastingSettingsCommand()
  {
    var settings = new BroadcastingSettings
                   {
                     UpdListenerPort = this.ListenerPort,
                     ConnectionPassword = this.ConnectionPassword,
                     CommandPassword = this.CommandPassword
                   };
    AccConfigProvider.SaveBroadcastingSettings(settings);
    this.FinishedSummaryVisibility = Visibility.Visible;
    this.BroadcastingSettingsVisibility = Visibility.Collapsed;
    this.IsFinishEnabled = true;
  }

  private void ProcessBroadcastingSettingsCheck()
  {
    this.IsNextEnabled = false;
    this.IsFinishEnabled = false;
    var settings = AccConfigProvider.GetBroadcastingSettings();
    if(settings?.UpdListenerPort > 0)
    {
      this.BroadcastingCheckSuccessPanelVisibility = Visibility.Visible;
      this.BroadcastingCheckFailPanelVisibility = Visibility.Collapsed;
      this.IsFinishEnabled = true;
      this.BroadcastingPort = settings.UpdListenerPort.ToString();
    }
    else
    {
      this.BroadcastingCheckSuccessPanelVisibility = Visibility.Collapsed;
      this.BroadcastingCheckFailPanelVisibility = Visibility.Visible;
      this.ChangeSettingsForMeVisibility = Visibility.Visible;
      this.OpenBroadcastingFileVisibility = Visibility.Visible;
      this.RetryBroadcastingCheckVisibility = Visibility.Collapsed;
    }
  }

  private void ProcessInstallationCheck()
  {
    this.IsNextEnabled = false;
    if(Directory.Exists(AccPathProvider.ConfigFolderPath))
    {
      this.InstallationCheckSuccessPanelVisibility = Visibility.Visible;
      this.InstallationCheckFailPanelVisibility = Visibility.Collapsed;
      this.IsNextEnabled = true;
    }
    else
    {
      this.InstallationCheckSuccessPanelVisibility = Visibility.Collapsed;
      this.InstallationCheckFailPanelVisibility = Visibility.Visible;
    }
  }

  private void ProcessSummaryOrSaveSettings()
  {
    this.IsNextEnabled = false;
    this.IsFinishEnabled = false;
    if(this.ChangeSettingsForMe)
    {
      this.FinishedSummaryVisibility = Visibility.Collapsed;
      this.BroadcastingSettingsVisibility = Visibility.Visible;
    }
    else
    {
      this.FinishedSummaryVisibility = Visibility.Visible;
      this.BroadcastingSettingsVisibility = Visibility.Collapsed;
      this.IsFinishEnabled = true;
    }
  }
}