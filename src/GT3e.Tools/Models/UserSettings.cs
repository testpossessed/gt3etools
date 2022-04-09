using System.Collections.Generic;

namespace GT3e.Tools.Models;

internal class UserSettings
{
    private readonly IList<string> customSkins = new List<string>();
    public IEnumerable<string> CustomSkins => this.customSkins;
    public string FullName => $"{this.FirstName} {this.LastName}";
    public string FirstName { get; set; }
    public bool IsFirstTimeInDashboard { get; set; } = true;
    public bool IsInitialised { get; set; } = false;
    public bool IsVerificationPending { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public string LastName { get; set; }
    public int RejectionCount { get; set; }
    public string SteamId { get; set; } = null!;
    public string Theme { get; set; } = null!;

    internal void AddCustomSkin(string name)
    {
        this.customSkins.Add(name);
    }

    internal bool IsUserSkin(string name)
    {
        return this.customSkins.IndexOf(name) >= 0;
    }
}