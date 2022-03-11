namespace GT3e.Tools.Acc.Data;

public struct BroadcastingEvent
{
    public BroadcastingEventType BroadcastingEventType { get; internal set; }
    public string Message { get; internal set; }
    public int TimeMs { get; internal set; }
    public int CarId { get; internal set; }
    public CarInfo CarData { get; internal set; }
}