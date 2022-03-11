namespace GT3e.Tools.Acc.Data;

public class EntryListUpdate
{
    public EntryListUpdate(string sender, CarInfo carInfo)
    {
        this.Sender = sender;
        this.CarInfo = carInfo;
    }

    public CarInfo CarInfo { get; }
    public string Sender { get; }
}