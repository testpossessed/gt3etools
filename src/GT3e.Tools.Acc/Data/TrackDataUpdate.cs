namespace GT3e.Tools.Acc.Data;

public class TrackDataUpdate
{
    public Dictionary<string, List<string>> CameraSets { get; internal set; }
    public IEnumerable<string> HudPages { get; internal set; }
    public string ConnectionIdentifier { get; internal set; }
    public int TrackId { get; internal set; }
    public float TrackMeters { get; internal set; }
    public string TrackName { get; internal set; }
}