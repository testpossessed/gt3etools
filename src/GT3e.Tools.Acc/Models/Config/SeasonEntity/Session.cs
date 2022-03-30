﻿namespace GT3e.Tools.Acc.Models.Config.SeasonEntity;

public class Session
{
    public int DateHour { get; set; }
    public int DateMinute { get; set; }
    public int RaceDay { get; set; }
    public double TimeMultiplier { get; set; }
    public int PreSessionDuration { get; set; }
    public long SessionDuration { get; set; }
    public int OvertimeDuration { get; set; }
    public int Round { get; set; }
    public int SessionType { get; set; }
    public double DynamicTrackMultiplier { get; set; }
    public TrackStatus TrackStatus { get; set; }
}