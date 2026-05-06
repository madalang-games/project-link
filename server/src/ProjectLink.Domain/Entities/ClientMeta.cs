namespace ProjectLink.Domain.Entities;

public class ClientMeta
{
    public string        ClientVersion   { get; set; } = default!;
    public string        MetaHash        { get; set; } = default!;
    public string        ProtocolVersion { get; set; } = default!;
    public DateTimeOffset CreatedAt      { get; set; }
}
