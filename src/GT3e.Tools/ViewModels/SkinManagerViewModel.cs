using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GT3e.Acc;
using GT3e.Acc.Models.Customs;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class SkinManagerViewModel : ObservableObject
{
    private CustomCar selectedCustomCar;

    public SkinManagerViewModel()
    {
        this.UploadCommand = new AsyncRelayCommand(this.HandleUploadCommand);
        var customCars = AccDataProvider.GetCustomCars();
        foreach(var customCar in customCars)
        {
            if(!string.IsNullOrWhiteSpace(customCar.CustomSkinName))
            {
                this.CustomCars.Add(customCar);
            }
        }
    }

    public ObservableCollection<CustomCar> CustomCars { get; } = new();

    public IAsyncRelayCommand UploadCommand { get; }
    public CustomCar SelectedCustomCar
    {
        get => this.selectedCustomCar;
        set => this.SetProperty(ref this.selectedCustomCar, value);
    }

    private Task HandleUploadCommand()
    {
        return null;
    }
}