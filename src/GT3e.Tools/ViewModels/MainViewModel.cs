using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        // this.InitialiseFileWatcher();
        this.SetInitialTheme();
        this.FirstTimeRun = new FirstTimeRunViewModel();
        this.ProcessInitialisation();
    }

    public FirstTimeRunViewModel FirstTimeRun { get; }

    public List<string> Themes { get; } = new()
    {
        "Blend",
        "Material Dark",
        "Office 2019 Dark Gray",
        "Office 365",
        "Saffron"
    };
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

    private void HandleThemeChanged()
    {
        var theme = this.SelectedTheme.Replace(" ", "");
        SettingsProvider.SaveSettings(new UserSettings
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

    private void InitialiseFileWatcher()
    {
        // this.fileWatcher = new FileSystemWatcher(PathProvider.AccSetupsFolderPath, "*.json")
        //                    {
        //                      IncludeSubdirectories = true,
        //                      EnableRaisingEvents = true
        //                    };
        // this.fileWatcher.Created += this.HandleSetupFileCreated;
        // this.fileWatcher.Deleted += this.HandleSetupFileDeleted;
        // this.fileWatcher.Changed += this.HandleSetupFileChanged;
    }

    private void ProcessInitialisation()
    {
        var settings = SettingsProvider.GetSettings();
        this.FirstTimeRun.FirstTimeRunVisibility =
            settings.IsInitialised? Visibility.Hidden: Visibility.Visible;
        this.FurnitureVisibility = settings.IsInitialised? Visibility.Visible: Visibility.Hidden;
        this.FirstTimeRun.PropertyChanged += HandleFirstTimeRunFinished;
    }

    private void HandleFirstTimeRunFinished(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if(eventArgs.PropertyName == nameof(FirstTimeRun.FirstTimeRunVisibility))
        {
            this.FurnitureVisibility = FirstTimeRun.FirstTimeRunVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }

    private void SetInitialTheme()
    {
        var settings = SettingsProvider.GetSettings();
        this.selectedTheme = settings.Theme;
        SfSkinManager.SetTheme(Application.Current.MainWindow,
            new Theme(this.SelectedTheme.Replace(" ", "")));
    }
}