using System.Windows;
using System.Windows.Input;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class VerificationTestViewModel : ObservableObject
{
    private bool isCancelEnabled;
    private bool isStartEnabled;
    private string steamId;
    private VerificationTestRunner verificationTestRunner;
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
        this.verificationTestRunner.Cancel();
    }

    private void HandleStartCommand()
    {
        this.IsStartEnabled = false;
        this.IsCancelEnabled = true;
        this.verificationTestRunner = new VerificationTestRunner();
        this.verificationTestRunner.Run(this.SteamId, this.HandleVerificationTestCallback);
    }

    private void HandleVerificationTestCallback(bool result)
    {
        if(result)
        {
            this.VerificationTestVisibility = Visibility.Collapsed;
        }
        else
        {
            this.IsCancelEnabled = false;
            this.IsStartEnabled = true;
        }
    }
}