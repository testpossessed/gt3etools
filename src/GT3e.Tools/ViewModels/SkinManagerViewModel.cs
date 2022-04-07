using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GT3e.Acc;
using GT3e.Acc.Models.Customs;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class SkinManagerViewModel : ObservableObject
{
    private CustomSkin selectedSkin;
    private bool isUploadEnabled;

    public SkinManagerViewModel()
    {
        this.UploadCommand = new AsyncRelayCommand(this.HandleUploadCommand);
        this.RefreshSkinsCommand = new RelayCommand(this.HandleRefreshSkinsCommand);
        this.LoadCustomSkins();
    }

    public ICommand RefreshSkinsCommand { get; }

    private void HandleRefreshSkinsCommand()
    {
        this.LoadCustomSkins();
    }

    private void LoadCustomSkins()
    {
        var customSkins = AccDataProvider.GetCustomSkins();
        this.Skins.Clear();
        foreach(var skin in customSkins)
        {
            this.Skins.Add(skin);
        }
    }

    public ObservableCollection<CustomSkin> Skins { get; } = new();

    public IAsyncRelayCommand UploadCommand { get; }
    public CustomSkin SelectedSkin
    {
        get => this.selectedSkin;
        set
        {
            this.SetProperty(ref this.selectedSkin, value);
            this.IsUploadEnabled = value != null;
        }
    }

    private async Task HandleUploadCommand()
    {
        await StorageProvider.UploadCustomSkin(this.SelectedSkin);
    }

    public bool IsUploadEnabled
    {
        get  => this.isUploadEnabled; 
        set => this.SetProperty(ref this.isUploadEnabled, value);
    }
}