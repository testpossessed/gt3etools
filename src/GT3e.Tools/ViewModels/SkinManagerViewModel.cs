using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Acc.Lib;
using Acc.Lib.Models.Customs;
using GT3e.Tools.Models;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class SkinManagerViewModel : ObservableObject
{
  private readonly List<CustomSkinInfo> newRemoteSkins = new();
  private readonly List<CustomSkinInfo> updatedRemoteSkins = new();
  private bool isSyncEnabled;
  private bool isUploadEnabled;
  private int newSkinCount;
  private Visibility noSkinsMessageVisibility;
  private CustomSkin selectedSkin;
  private int updatedSkinCount;

  public SkinManagerViewModel()
  {
    this.UploadCommand = new AsyncRelayCommand(this.HandleUploadCommand);
    this.RefreshLocalSkinsCommand = new RelayCommand(this.HandleRefreshLocalSkinsCommand);
    this.SyncSkinsCommand = new AsyncRelayCommand(this.HandleSyncSkinsCommand);
    this.RefreshRemoteSkinsCommand = new RelayCommand(this.HandleRemoteLocalSkinsCommand);

    this.LoadLocalCustomSkins();
    this.LoadRemoteCustomSkins();
  }

  public ICommand RefreshLocalSkinsCommand { get; }

  public ICommand RefreshRemoteSkinsCommand { get; }

  public ObservableCollection<CustomSkin> Skins { get; } = new();

  public IAsyncRelayCommand SyncSkinsCommand { get; }

  public IAsyncRelayCommand UploadCommand { get; }

  public bool IsSyncEnabled
  {
    get => this.isSyncEnabled;
    set => this.SetProperty(ref this.isSyncEnabled, value);
  }

  public bool IsUploadEnabled
  {
    get => this.isUploadEnabled;
    set => this.SetProperty(ref this.isUploadEnabled, value);
  }

  public int NewSkinCount
  {
    get => this.newSkinCount;
    set => this.SetProperty(ref this.newSkinCount, value);
  }

  public Visibility NoSkinsMessageVisibility
  {
    get => this.noSkinsMessageVisibility;
    set => this.SetProperty(ref this.noSkinsMessageVisibility, value);
  }
  public CustomSkin SelectedSkin
  {
    get => this.selectedSkin;
    set
    {
      this.SetProperty(ref this.selectedSkin, value);
      this.IsUploadEnabled = value != null;
    }
  }

  public int UpdatedSkinCount
  {
    get => this.updatedSkinCount;
    set => this.SetProperty(ref this.updatedSkinCount, value);
  }

  private void HandleRefreshLocalSkinsCommand()
  {
    this.LoadLocalCustomSkins();
  }

  private void HandleRemoteLocalSkinsCommand()
  {
    this.LoadRemoteCustomSkins();
  }

  private async Task HandleSyncSkinsCommand()
  {
    var skinsToSync = this.newRemoteSkins.Concat(this.updatedRemoteSkins)
                          .ToList();

    foreach(var skinInfo in skinsToSync)
    {
      var zipFilePath = await StorageProvider.DownloadCustomSkin(skinInfo);
      FilePackager.UnpackCustomSkin(zipFilePath);
    }

    this.LoadLocalCustomSkins();
    this.LoadRemoteCustomSkins();
  }

  private async Task HandleUploadCommand()
  {
    await StorageProvider.UploadCustomSkin(this.SelectedSkin);
    var userSettings = SettingsProvider.GetUserSettings();
    userSettings.AddCustomSkin(this.SelectedSkin.Name);
    SettingsProvider.SaveUserSettings(userSettings);
  }

  private void LoadLocalCustomSkins()
  {
    var customSkins = AccDataProvider.GetCustomSkins();
    this.Skins.Clear();
    foreach(var skin in customSkins)
    {
      this.Skins.Add(skin);
    }

    if(this.Skins.Any())
    {
      this.NoSkinsMessageVisibility = Visibility.Collapsed;
      return;
    }

    this.NoSkinsMessageVisibility = Visibility.Visible;
  }

  private void LoadRemoteCustomSkins()
  {
    this.newRemoteSkins.Clear();
    this.updatedRemoteSkins.Clear();

    var remoteSkins = StorageProvider.GetCustomSkins()
                                     .GetAwaiter()
                                     .GetResult();
    var userSettings = SettingsProvider.GetUserSettings();
    foreach(var remoteSkin in remoteSkins)
    {
      if(userSettings.IsUserSkin(remoteSkin.Name))
      {
        continue;
      }

      var localSkin = this.Skins.FirstOrDefault(s => s.Name == remoteSkin.Name);
      if(localSkin == null)
      {
        this.newRemoteSkins.Add(remoteSkin);
      }
      else if(remoteSkin.LastModifiedUtc.GetValueOrDefault() > localSkin.LastModifiedUtc)
      {
        this.updatedRemoteSkins.Add(remoteSkin);
      }
    }

    this.NewSkinCount = this.newRemoteSkins.Count;
    this.UpdatedSkinCount = this.updatedRemoteSkins.Count;
    this.IsSyncEnabled = this.newSkinCount + this.updatedSkinCount > 0;
  }
}