using System.Windows;
using System.Windows.Input;
using GT3e.Tools.Models;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class DashboardViewModel : ObservableRecipient
{
    private readonly UserSettings userSettings;
    private Visibility dashboardVisibility;
    private string driverName;
    private Visibility firstTimeRunVisibility;

    public DashboardViewModel()
    {
        this.DashboardVisibility = Visibility.Collapsed;
        this.HideFirstTimeDashboardSectionCommand =
            new RelayCommand(this.HandleHideFirstTimeDashboardSectionCommand);
        this.userSettings = SettingsProvider.GetUserSettings();
        this.DriverName = this.userSettings.FullName;
        this.FirstTimeRunVisibility =
            this.userSettings.IsFirstTimeInDashboard? Visibility.Visible: Visibility.Collapsed;
    }

    public ICommand HideFirstTimeDashboardSectionCommand { get; }

    public SkinManagerViewModel SkinManager { get; } = new();
    public Visibility DashboardVisibility
    {
        get => this.dashboardVisibility;
        set => this.SetProperty(ref this.dashboardVisibility, value);
    }

    public string DriverName { get => this.driverName; set => this.SetProperty(ref this.driverName, value); }

    public Visibility FirstTimeRunVisibility
    {
        get => this.firstTimeRunVisibility;
        set => this.SetProperty(ref this.firstTimeRunVisibility, value);
    }

    private void HandleHideFirstTimeDashboardSectionCommand()
    {
        this.userSettings.IsFirstTimeInDashboard = false;
        this.FirstTimeRunVisibility = Visibility.Collapsed;
        SettingsProvider.SaveUserSettings(this.userSettings);
    }
}