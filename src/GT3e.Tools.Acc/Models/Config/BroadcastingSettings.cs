namespace GT3e.Tools.Acc.Models.Config;

public class BroadcastingSettings
{
    public string CommandPassword { get; set; } = null!;
    public string ConnectionPassword { get; set; } = null!;
    public int UpdListenerPort { get; set; }
}