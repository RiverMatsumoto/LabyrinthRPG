using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed class UseRequest
{
    public required UseContext Context { get; init; }
    public required Battler Source { get; init; }
    public Battler Target { get; init; }
    public object WorldState { get; init; }
}

public sealed class UseResult
{
    public bool Consumed { get; init; }
    public string Error { get; init; }

    public static UseResult Ok(bool consumed) => new() { Consumed = consumed };
    public static UseResult Fail(string error) => new() { Consumed = false, Error = error };
}

[GlobalClass]
public partial class UseModule : ItemModule
{
    [Export] public Godot.Collections.Array<ItemUseAction> Actions { get; set; } = new();

    public IEnumerable<ItemUseAction> GetValidActions(UseContext ctx)
        => Actions.Where(a => a.AllowedContexts.Contains(ctx));
}

[GlobalClass]
public partial class ItemUseAction : Resource
{
    [Export] public string ActionId { get; set; } = "use";
    [Export] public string DisplayName { get; set; } = "Use";
    [Export] public Targeting Targeting { get; set; } = Targeting.Self;
    [Export]
    public Godot.Collections.Array<UseContext> AllowedContexts { get; set; } = new()
    {
        UseContext.Labyrinth,
        UseContext.Battle
    };
    [Export] public bool RequiresBattleTarget { get; set; } = false;
    [Export] public Godot.Collections.Array<ItemEffect> Effects { get; set; } = new();

    public UseResult TryExecute(UseRequest req)
    {
        if (!AllowedContexts.Contains(req.Context))
            return UseResult.Fail("Cannot use item in this context.");

        if (RequiresBattleTarget && req.Target is null)
            return UseResult.Fail("No target selected.");

        foreach (var e in Effects)
        {
            var r = e.Apply(req);
            if (!r.Success) return UseResult.Fail(r.Error ?? "Item use failed.");
        }

        // Consumable actions default to consumed = true
        return UseResult.Ok(consumed: true);
    }
}

public readonly record struct EffectResult(bool Success, string Error = null)
{
    public static EffectResult Ok() => new(true, null);
    public static EffectResult Fail(string error) => new(false, error);
}

public abstract partial class ItemEffect : Resource
{
    public abstract EffectResult Apply(UseRequest req);
}

// Example effect: heal flat amount
[GlobalClass]
public partial class HealEffect : ItemEffect
{
    [Export] public int Amount { get; set; } = 50;

    public override EffectResult Apply(UseRequest req)
    {
        if (req.Target is null) return EffectResult.Fail("No target.");
        // apply healing directly to the Battler's stats
        req.Target.Stats.Hp += Amount;
        return EffectResult.Ok();
    }
}
