namespace ProjectLink.Domain.Entities;

public class StageProgress
{
    public string         UserId    { get; set; } = default!;
    public int            StageId   { get; set; }
    public int            Stars     { get; set; }
    public DateTimeOffset ClearedAt { get; set; }
}
