namespace GT3e.Tools.Acc.Data;

public enum InboundMessageTypes : byte
{
    RegistrationResult = 1,
    RealtimeUpdate = 2,
    RealtimeCarUpdate = 3,
    EntryList = 4,
    EntryListCar = 6,
    TrackData = 5,
    BroadcastingEvent = 7
}