using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class VerificationTestViewModel : ObservableObject
{
    private Visibility verificationTestVisibility;

    public VerificationTestViewModel()
    {
        this.StartCommand = new RelayCommand(this.HandleStartCommand);
    }

    private void HandleStartCommand()
    {

    }

    public ICommand StartCommand { get; }

    public Visibility VerificationTestVisibility
    {
        get => this.verificationTestVisibility;
        set => this.SetProperty(ref this.verificationTestVisibility, value);
    }
}