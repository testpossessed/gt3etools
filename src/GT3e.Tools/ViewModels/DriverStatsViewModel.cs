using System.Windows;
using GT3e.Tools.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class DriverStatsViewModel : ObservableObject
{
    private readonly DriverStats driverStats;
    private Visibility statsPanelVisibility;

    public DriverStatsViewModel(DriverStats driverStats)
    {
        this.driverStats = driverStats;
        this.RefreshCommand = new RelayCommand(this.HandleRefreshCommand);
        this.StatsPanelVisibility = Visibility.Collapsed;
        this.HandleRefreshCommand();
    }

    public IRelayCommand RefreshCommand { get; }

    public Visibility StatsPanelVisibility
    {
        get => this.statsPanelVisibility;
        set => this.SetProperty(ref this.statsPanelVisibility, value);
    }

    private void HandleRefreshCommand() { }
}