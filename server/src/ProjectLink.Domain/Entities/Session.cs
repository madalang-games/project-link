namespace ProjectLink.Domain.Entities;

public class Session
{
    public long             Id        { get; set; }
    public string           UserId    { get; set; } = default!;
    public string           SessionId { get; set; } = default!;
    public DateTimeOffset   CreatedAt { get; set; }
    public DateTimeOffset   ExpiresAt { get; set; }
    public bool             Active    { get; set; }
}
