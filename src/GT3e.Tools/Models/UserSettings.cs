namespace GT3e.Tools.Models;

internal class UserSettings
{
    public string FullName => $"{this.FirstName} {this.LastName}";
    public string FirstName { get; set; }
    public bool IsFirstTimeInDashboard { get; set; } = true;
    public bool IsInitialised { get; set; } = false;
    public bool IsVerificationPending { get; set; } = false;
    public int RejectionCount { get; set; }
    public bool IsVerified { get; set; } = false;
    public string LastName { get; set; }
    public string SteamId { get; set; } = null!;
    public string Theme { get; set; } = null!;
}