namespace GT3e.Tools.Models;

public class BroadcastingSettings
{
    public string CommandPassword { get; set; } = null!;
    public string ConnectionPassword { get; set; } = null!;
    public int UpdListenerPort { get; set; }
}