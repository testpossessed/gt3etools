namespace GT3e.Tools.Acc.Models.RaceResult;

public class SnapShot
{
    public int Bestlap { get; set; }
    public List<int> BestSplits { get; set; }
    public int IsWetSession { get; set; }
    public int Type { get; set; }
    public List<LeaderBoardLine> LeaderBoardLines { get; set; }
}