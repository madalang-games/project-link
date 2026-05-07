namespace ProjectLink.Domain.Entities;

public class PlayerSettings
{
    public string         UserId               { get; set; } = default!;
    public bool           BgmEnabled           { get; set; } = true;
    public bool           SfxEnabled           { get; set; } = true;
    public bool           HapticsEnabled       { get; set; } = true;
    public bool           NotificationsEnabled { get; set; } = true;
    public string         Language             { get; set; } = "EN";
    public DateTimeOffset UpdatedAt            { get; set; }
}
