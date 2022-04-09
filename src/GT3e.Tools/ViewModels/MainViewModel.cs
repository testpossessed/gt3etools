using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using GT3e.Tools.Models;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Syncfusion.SfSkinManager;

namespace GT3e.Tools.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private Visibility furnitureVisibility;
    private string selectedTheme;
    private string statusMessage = "Started";

    public MainViewModel()
    {
        this.SetInitialTheme();
        this.Initialise();
    }

    public ConsoleViewModel Console { get; } = new();

    public DashboardViewModel Dashboard { get; } = new();

    public FirstTimeRunViewModel FirstTimeRun { get; } = new();

    public List<string> Themes { get; } = new()
    {
        "Blend",
        "Material Dark",
        "Office 2019 Dark Gray",
        "Office 365",
        "Saffron"
    };

    public VerificationPendingViewModel VerificationPending { get; } = new();

    public VerificationTestViewModel VerificationTest { get; } = new();

    public Visibility FurnitureVisibility
    {
        get => this.furnitureVisibility;
        set => this.SetProperty(ref this.furnitureVisibility, value);
    }

    public string SelectedTheme
    {
        get => this.selectedTheme;
        set
        {
            this.SetProperty(ref this.selectedTheme, value);
            this.HandleThemeChanged();
        }
    }

    public string StatusMessage
    {
        get => this.statusMessage;
        set => this.SetProperty(ref this.statusMessage, value);
    }

    private void HandleFirstTimeRunFinished(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if(eventArgs.PropertyName != nameof(this.FirstTimeRun.FirstTimeRunVisibility))
        {
            return;
        }

        var contentVisibility = this.FirstTimeRun.FirstTimeRunVisibility == Visibility.Visible
                                    ? Visibility.Collapsed
                                    : Visibility.Visible;
        this.FurnitureVisibility = contentVisibility;
        this.Console.ConsoleVisibility = contentVisibility;
        this.VerificationTest.VerificationTestVisibility = contentVisibility;
        this.FirstTimeRun.PropertyChanged -= this.HandleFirstTimeRunFinished;
        this.VerificationTest.PropertyChanged += this.HandleVerificationTestFinished;
    }

    private void HandleThemeChanged()
    {
        var theme = this.SelectedTheme.Replace(" ", "");
        SettingsProvider.SaveUserSettings(new UserSettings
        {
            Theme = this.SelectedTheme
        });
        var result =
            MessageBox.Show(
                $"The theme will be applied when the application next starts.{Environment.NewLine}{Environment.NewLine}Do you want to exit now?",
                "Theme Change",
                MessageBoxButton.YesNo);

        if(result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown(0);
        }
    }

    private void HandleVerificationPendingFinished(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if(eventArgs.PropertyName != nameof(this.VerificationPending.VerificationPendingVisibility))
        {
            return;
        }

        if(this.VerificationPending.VerificationPendingVisibility != Visibility.Collapsed)
        {
            return;
        }

        this.Dashboard.DashboardVisibility = Visibility.Visible;
        this.VerificationPending.PropertyChanged -= this.HandleVerificationPendingFinished;
    }

    private void HandleVerificationTestFinished(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if(eventArgs.PropertyName != nameof(this.VerificationTest.VerificationTestVisibility))
        {
            return;
        }

        var contentVisibility = this.VerificationTest.VerificationTestVisibility == Visibility.Visible
                                    ? Visibility.Collapsed
                                    : Visibility.Visible;
        this.VerificationPending.VerificationPendingVisibility = contentVisibility;
    }

    private void Initialise()
    {
        var userSettings = SettingsProvider.GetUserSettings();
        if(!Directory.Exists(PathProvider.SyncDownloadsFolderPath))
        {
            Directory.CreateDirectory(PathProvider.SyncDownloadsFolderPath);
        }
        this.PrepareFirstTimeRun(userSettings);
        this.PrepareVerificationTest(userSettings);
        this.PrepareVerificationPending(userSettings);
        this.PrepareDashboard(userSettings);
    }

    private void PrepareDashboard(UserSettings userSettings)
    {
        if(userSettings.IsVerified)
        {
            this.Dashboard.DashboardVisibility = Visibility.Visible;
        }
    }

    private void PrepareFirstTimeRun(UserSettings userSettings)
    {
        this.FirstTimeRun.FirstTimeRunVisibility =
            userSettings.IsInitialised? Visibility.Collapsed: Visibility.Visible;
        var contentVisibility = userSettings.IsInitialised? Visibility.Visible: Visibility.Collapsed;
        this.FurnitureVisibility = contentVisibility;
        this.Console.ConsoleVisibility = contentVisibility;
        if(!userSettings.IsInitialised)
        {
            this.FirstTimeRun.PropertyChanged += this.HandleFirstTimeRunFinished;
        }
    }

    private void PrepareVerificationPending(UserSettings userSettings)
    {
        var visibility = Visibility.Collapsed;
        if(userSettings.IsInitialised && !userSettings.IsVerified && userSettings.IsVerificationPending)
        {
            var driverStats = StorageProvider.GetDriverStats(userSettings.SteamId);
            var hasAcceptance = driverStats?.VerificationTestAttempts.Any(e => e.Rejected == false);
            var isVerified = hasAcceptance.GetValueOrDefault();
            visibility = isVerified? Visibility.Collapsed: Visibility.Visible;

            if(isVerified)
            {
                userSettings.IsVerificationPending = false;
                userSettings.IsVerified = true;
                userSettings.IsFirstTimeInDashboard = true;
                userSettings.FirstName = driverStats!.FirstName;
                userSettings.LastName = driverStats.LastName;

                SettingsProvider.SaveUserSettings(userSettings);
            }
            else
            {
                this.VerificationPending.PropertyChanged += this.HandleVerificationPendingFinished;
            }
        }

        this.VerificationPending.VerificationPendingVisibility = visibility;
    }

    private void PrepareVerificationTest(UserSettings userSettings)
    {
        var visibility = Visibility.Collapsed;
        var showVerificationTest = userSettings.IsInitialised && !userSettings.IsVerified
                                                              && !userSettings.IsVerificationPending;
        if(showVerificationTest)
        {
            visibility = Visibility.Visible;
            this.VerificationTest.PropertyChanged += this.HandleVerificationTestFinished;
        }

        this.VerificationTest.VerificationTestVisibility = visibility;
    }

    private void SetInitialTheme()
    {
        var settings = SettingsProvider.GetUserSettings();
        this.selectedTheme = settings.Theme;
        SfSkinManager.SetTheme(Application.Current.MainWindow,
            new Theme(this.SelectedTheme.Replace(" ", "")));
    }
}