using System.Collections.Generic;
using System.Threading;
using Godot;

public partial class TestBattleSystemNode : Node
{
    public override async void _Ready()
    {
        GD.Print("=== Battle System Test Start ===");

        // Load actions
        var library = new ActionLibrary();

        // Fake battle context
        var source = new Battler();
        var target = new Battler();

        var ctx = new BattleRunCtx(
            model: new BattleModel(),
            source: source,
            targets: new[] { target },
            damageRegistry: new TestDamageRegistry(),
            animator: new TestAnimator(),
            damagePopup: new TestDamagePopup()
        );

        // Execute
        var actions = new List<ActionDef>();
        actions.Add(library.Get("BasicAttack"));
        var actionCt = new CancellationTokenSource();
        foreach (var a in actions)
        {
            // debug print
            GD.Print($"[Debug] basic attack effects: {a.Effects.ToString()}");
            await ActionExecutor.ExecuteAsync(a, ctx, actionCt.Token);
        }

        GD.Print("=== Battle System Test End ===");
    }
}
