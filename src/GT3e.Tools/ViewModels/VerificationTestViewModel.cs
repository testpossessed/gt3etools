using System;
using System.Windows;
using System.Windows.Input;
using GT3e.Tools.Acc;
using GT3e.Tools.Acc.Data;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GT3e.Tools.ViewModels;

public class VerificationTestViewModel : ObservableObject
{
    private bool isStartEnabled;
    private Visibility verificationTestVisibility;

    public VerificationTestViewModel()
    {
        this.StartCommand = new RelayCommand(this.HandleStartCommand);
        this.IsStartEnabled = true;
    }

    public ICommand StartCommand { get; }

    public bool IsStartEnabled
    {
        get => this.isStartEnabled;
        set => this.SetProperty(ref this.isStartEnabled, value);
    }

    public Visibility VerificationTestVisibility
    {
        get => this.verificationTestVisibility;
        set => this.SetProperty(ref this.verificationTestVisibility, value);
    }

    private void HandleAccError(Exception exception)
    {
        ConsoleLog.Write($"ERROR: {exception.Message}");
    }

    private void HandleBroadcastingEvents(BroadcastingEvent broadcastingEvent)
    {
        ConsoleLog.Write($"Broadcasting Event: Message: {broadcastingEvent.Message}");
    }

    private void HandleConnectionStatusUpdates(ConnectionState connectionState)
    {
        ConsoleLog.Write(string.IsNullOrWhiteSpace(connectionState.Error)
                             ? $"Connection State: ID: {connectionState.ConnectionId} Connected: {connectionState.IsConnected} Read Only: {connectionState.IsReadOnly}"
                             : $"Connection State: ID: {connectionState.ConnectionId} ERROR: {connectionState.Error}");
    }

    private void HandleEntryListUpdates(EntryListUpdate entryListUpdate)
    {
        ConsoleLog.Write($"Entry List Update: Race Number: {entryListUpdate.CarInfo.RaceNumber}");
    }

    private void HandleRealTimeCarUpdates(RealtimeCarUpdate update)
    {
        ConsoleLog.Write($"Real Time Car Update: CarLocation: {update.CarLocation}");
    }

    private void HandleRealTimeUpdates(RealtimeUpdate update)
    {
        ConsoleLog.Write($"Real Time Update: Session Type: {update.SessionType}");
    }

    private void HandleStartCommand()
    {
        this.IsStartEnabled = false;
        var broadcastingSettings = AccConfigProvider.GetBroadcastingSettings()!;
        var accConnection = new AccConnection("localhost",
            broadcastingSettings.UpdListenerPort,
            "Verification Test",
            broadcastingSettings.ConnectionPassword,
            broadcastingSettings.ConnectionPassword,
            60);

        accConnection.BroadcastingEvents.Subscribe(this.HandleBroadcastingEvents, this.HandleAccError);
        accConnection.ConnectionStateChanges.Subscribe(this.HandleConnectionStatusUpdates,
            this.HandleAccError);
        accConnection.EntryListUpdates.Subscribe(this.HandleEntryListUpdates, this.HandleAccError);
        accConnection.RealTimeUpdates.Subscribe(this.HandleRealTimeUpdates, this.HandleAccError);
        accConnection.RealTimeCarUpdates.Subscribe(this.HandleRealTimeCarUpdates, this.HandleAccError);
        accConnection.TrackDataUpdates.Subscribe(this.HandleTrackDataUpdates, this.HandleAccError);

        ConsoleLog.Write("Connecting to ACC...");

        accConnection.Connect();
    }

    private void HandleTrackDataUpdates(TrackDataUpdate update)
    {
        ConsoleLog.Write($"Track Data Update: Track Name: {update.TrackName}");
    }
}