using System.Linq;
using System.Windows;
using System.Windows.Input;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class VerificationPendingViewModel : ObservableObject
{
    private bool isCancelRetryEnabled;
    private bool isRetryEnabled;
    private Visibility attemptPendingVisibility;
    private Visibility attemptRejectedMessageVisibility;
    private string rejectionReason;
    private int remainingAttempts;
    private Visibility verificationPendingVisibility;

    public VerificationPendingViewModel()
    {
        this.AttemptPendingVisibility = Visibility.Visible;
        this.AttemptRejectedMessageVisibility = Visibility.Collapsed;
        this.CheckStatusCommand = new RelayCommand(this.HandleCheckStatusCommand);
        this.TryAgainCommand = new RelayCommand(this.HandleRetryCommand);
        this.CancelRetryCommand = new RelayCommand(this.HandleCancelRetryCommand);
        this.HandleCheckStatusCommand();
    }

    public ICommand CancelRetryCommand { get; }

    public ICommand CheckStatusCommand { get; }

    public bool IsCancelRetryEnabled
    {
        get => this.isCancelRetryEnabled;
        set => this.SetProperty(ref this.isCancelRetryEnabled, value);
    }

    public bool IsRetryEnabled
    {
        get => this.isRetryEnabled;
        set => this.SetProperty(ref this.isRetryEnabled, value);
    }

    public Visibility AttemptPendingVisibility
    {
        get => this.attemptPendingVisibility;
        set => this.SetProperty(ref this.attemptPendingVisibility, value);
    }

    public Visibility AttemptRejectedMessageVisibility
    {
        get => this.attemptRejectedMessageVisibility;
        set => this.SetProperty(ref this.attemptRejectedMessageVisibility, value);
    }

    public string RejectionReason
    {
        get => this.rejectionReason;
        set => this.SetProperty(ref this.rejectionReason, value);
    }

    public int RemainingAttempts
    {
        get => this.remainingAttempts;
        set => this.SetProperty(ref this.remainingAttempts, value);
    }

    public ICommand TryAgainCommand { get; set; }
    public Visibility VerificationPendingVisibility
    {
        get => this.verificationPendingVisibility;
        set => this.SetProperty(ref this.verificationPendingVisibility, value);
    }

    private void HandleCancelRetryCommand() { }

    private void HandleCheckStatusCommand()
    {
        var userSettings = SettingsProvider.GetUserSettings();
        var riderStats = StorageProvider.GetDriverStats(userSettings.SteamId);
        if(riderStats == null)
        {
            return;
        }

        var hasBeenAccepted = riderStats.VerificationTestAttempts.Any(e => e.Rejected == false);
        if(hasBeenAccepted)
        {
            userSettings.IsVerificationPending = false;
            userSettings.IsVerified = true;
            SettingsProvider.SaveUserSettings(userSettings);
            this.VerificationPendingVisibility = Visibility.Collapsed;
            return;
        }

        var rejectionCount = riderStats.VerificationTestAttempts.Count(e => e.Rejected);
        if(rejectionCount > userSettings.RejectionCount)
        {
            this.AttemptRejectedMessageVisibility = Visibility.Visible;
            this.RemainingAttempts = 3 - riderStats.VerificationTestAttempts.Count;
            this.AttemptPendingVisibility = Visibility.Collapsed;
            this.RejectionReason = riderStats.VerificationTestAttempts.Last()
                                             .RejectionReason;
            this.IsRetryEnabled = this.RemainingAttempts > 0;
            userSettings.RejectionCount++;
            SettingsProvider.SaveUserSettings(userSettings);

            return;
        }

        this.AttemptRejectedMessageVisibility = Visibility.Collapsed;
        this.AttemptPendingVisibility = Visibility.Visible;
    }

    private void HandleRetryCommand()
    {
        this.IsRetryEnabled = false;
        this.IsCancelRetryEnabled = true;

        var userSettings = SettingsProvider.GetUserSettings();
        var verificationTestRunner = new VerificationTestRunner();
        verificationTestRunner.Run(userSettings.SteamId, this.HandleVerificationTestCallback);
    }

    private void HandleVerificationTestCallback(bool result)
    {
        var userSettings = SettingsProvider.GetUserSettings();
        userSettings.IsVerificationPending = false;
        SettingsProvider.SaveUserSettings(userSettings);

        if(result)
        {
            this.IsRetryEnabled = false;
            this.IsCancelRetryEnabled = false;
            this.AttemptRejectedMessageVisibility = Visibility.Hidden;
            this.AttemptPendingVisibility = Visibility.Visible;
            return;
        }

        this.IsRetryEnabled = true;
        this.IsCancelRetryEnabled = false;
    }
}