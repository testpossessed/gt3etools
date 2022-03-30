using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class VerificationPendingViewModel : ObservableObject
{
    public VerificationPendingViewModel()
    {
        this.CheckStatusCommand = new RelayCommand(this.HandleCheckStatusCommand);
    }

    public ICommand CheckStatusCommand { get; }
    public Visibility VerificationPendingVisibility { get; set; }
    private void HandleCheckStatusCommand() { }
}