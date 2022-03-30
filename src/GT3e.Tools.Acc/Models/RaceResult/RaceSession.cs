namespace GT3e.Tools.Acc.Models.RaceResult;

public class RaceSession
{
    public bool HasBeenSkipped { get; set; }
    public List<Lap> Laps { get; set; }
    public SessionDef SessionDef { get; set; }
    public SnapShot SnapShot { get; set; }
}