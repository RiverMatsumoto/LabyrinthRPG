
public sealed class QueuedAction
{
    public required Battler Source { get; init; }
    public required string ActionId { get; init; }
    public required object TargetSelection { get; init; } // can be a battler index, row, etc.
    public int Priority { get; init; }                     // speed/initiative result
}
