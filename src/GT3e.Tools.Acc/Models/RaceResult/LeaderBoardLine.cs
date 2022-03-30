namespace GT3e.Tools.Acc.Models.RaceResult;

public class LeaderBoardLine
{
    public Car Car { get; set; }
    public CurrentDriver CurrentDriver { get; set; }
    public int CurrentDriverIndex { get; set; }
    public Timing Timing { get; set; }
    public int MissingMandatoryPitstop { get; set; }
    public List<double> DriverTotalTimes { get; set; }
}