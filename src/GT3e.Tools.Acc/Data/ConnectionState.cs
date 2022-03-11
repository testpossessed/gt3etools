namespace GT3e.Tools.Acc.Data;

public class ConnectionState
{
    public ConnectionState(int connectionId, bool isConnected, bool isReadOnly, string? error = null)
    {
        ConnectionId = connectionId;
        IsConnected = isConnected;
        IsReadOnly = isReadOnly;
        Error = error;
    }

    public int ConnectionId { get; }
    public bool IsConnected { get; }
    public bool IsReadOnly { get; }
    public string? Error { get; }
}