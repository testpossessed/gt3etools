namespace GT3e.Tools.Acc.Models.RaceResult;

public class Timing
{
    public int LastLap { get; set; }
    public List<int> LastSplits { get; set; }
    public int BestLap { get; set; }
    public List<int> BestSplits { get; set; }
    public int TotalTime { get; set; }
    public int LapCount { get; set; }
    public int LastSplitId { get; set; }
}