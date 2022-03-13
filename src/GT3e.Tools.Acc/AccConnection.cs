using System.Diagnostics;
using System.Net.Sockets;
using GT3e.Tools.Acc.Data;

namespace GT3e.Tools.Acc;

public class AccConnection
{
    private readonly Task listenerTask;
    private readonly AccMessageHandler messageHandler;
    private bool isDisposed;
    private UdpClient? udpClient;

    public AccConnection(string ipAddress,
        int port,
        string displayName,
        string connectionPassword,
        string commandPassword,
        int updateInterval)
    {
        this.IpAddress = ipAddress;
        this.Port = port;
        this.DisplayName = displayName;
        this.ConnectionPassword = connectionPassword;
        this.CommandPassword = commandPassword;
        this.UpdateInterval = updateInterval;
        this.ConnectionIdentifier = $"{this.IpAddress}:{this.Port}";

        this.udpClient = new UdpClient();
        this.messageHandler = new AccMessageHandler(this.ConnectionIdentifier, this.Send);
        this.listenerTask = this.HandleMessages();
    }

    public string CommandPassword { get; }
    public string ConnectionIdentifier { get; }
    public string DisplayName { get; }
    public string IpAddress { get; }
    public int Port { get; }
    public int UpdateInterval { get; }
    public string ConnectionPassword { get; }

    public IObservable<BroadcastingEvent> BroadcastingEvents => this.messageHandler.BroadcastingEvents;
    public IObservable<ConnectionState> ConnectionStateChanges => this.messageHandler.ConnectionStateChanges;
    public IObservable<EntryListUpdate> EntryListUpdates => this.messageHandler.EntryListUpdates;
    public IObservable<RealtimeCarUpdate> RealTimeCarUpdates => this.messageHandler.RealTimeCarUpdates;
    public IObservable<RealtimeUpdate> RealTimeUpdates => this.messageHandler.RealTimeUpdates;
    public IObservable<TrackDataUpdate> TrackDataUpdates => this.messageHandler.TrackDataUpdates;

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~ACCUdpRemoteClient() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    public void Dispose()
    {
        this.Dispose(true);
        // GC.SuppressFinalize(this);
    }

    public void Connect()
    {
        this.udpClient!.Connect(this.IpAddress, this.Port);
        this.messageHandler.RequestConnection(this.DisplayName, this.ConnectionPassword, this.UpdateInterval, this.CommandPassword);
    }

    public async Task ShutdownAsync()
    {
        if(this.listenerTask is {IsCompleted: false})
        {
            this.messageHandler.Disconnect();
            this.udpClient?.Close();
            this.udpClient = null;
            await this.listenerTask;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if(this.isDisposed)
        {
            return;
        }

        if(disposing)
        {
            try
            {
                if(this.udpClient != null)
                {
                    this.udpClient.Close();
                    this.udpClient.Dispose();
                    this.udpClient = null;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        this.isDisposed = true;
    }

    private async Task HandleMessages()
    {
        this.messageHandler.RequestConnection(this.DisplayName,
            this.ConnectionPassword,
            this.UpdateInterval,
            this.CommandPassword);
        while(this.udpClient != null)
        {
            try
            {
                var udpPacket = await this.udpClient.ReceiveAsync();
                await using var stream = new MemoryStream(udpPacket.Buffer);
                using var reader = new BinaryReader(stream);
                this.messageHandler.ProcessMessage(reader);
            }
            catch(ObjectDisposedException)
            {
                break;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    private void Send(byte[] payload)
    {
        this.udpClient?.Send(payload);
    }
}