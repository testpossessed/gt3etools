using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GT3e.Acc;
using GT3e.Acc.Models.Customs;
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
    private CustomSkin selectedSkin;
    private int updatedSkinCount;

    public SkinManagerViewModel()
    {
        this.UploadCommand = new AsyncRelayCommand(this.HandleUploadCommand);
        this.RefreshLocalSkinsCommand = new RelayCommand(this.HandleRefreshLocalSkinsCommand);
        this.SyncSkinsCommand = new AsyncRelayCommand(this.HandleSyncSkinsCommand);

        this.LoadLocalCustomSkins();
        this.LoadRemoteCustomSkins();
    }

    public ICommand RefreshLocalSkinsCommand { get; }

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

    private Task HandleSyncSkinsCommand()
    {
        return null;
    }

    private async Task HandleUploadCommand()
    {
        await StorageProvider.UploadCustomSkin(this.SelectedSkin);
    }

    private void LoadLocalCustomSkins()
    {
        var customSkins = AccDataProvider.GetCustomSkins();
        this.Skins.Clear();
        foreach(var skin in customSkins)
        {
            this.Skins.Add(skin);
        }
    }

    private void LoadRemoteCustomSkins()
    {
        this.newRemoteSkins.Clear();
        this.updatedRemoteSkins.Clear();

        var remoteSkins = StorageProvider.GetCustomSkins()
                                         .GetAwaiter()
                                         .GetResult();
        foreach(var remoteSkin in remoteSkins)
        {
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

        this.newSkinCount = this.newRemoteSkins.Count;
        this.updatedSkinCount = this.updatedRemoteSkins.Count;
        this.IsSyncEnabled = this.newSkinCount + this.updatedSkinCount > 0;
    }
}