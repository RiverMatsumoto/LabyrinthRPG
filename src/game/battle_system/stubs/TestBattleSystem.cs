
using System.Threading;
using System.Threading.Tasks;
using Godot;

public sealed class TestAnimator : IAnimator
{
    public void Play(string anim)
    {
        GD.Print($"[Animator] Play (bg): {anim}");
    }

    public async Task PlayAsync(string anim, CancellationToken ct)
    {
        GD.Print($"[Animator] PlayAsync (wait): {anim}");
        await Task.Delay(300, ct); // simulate animation length
        GD.Print($"[Animator] Finished: {anim}");
    }
}

public sealed class TestDamagePopup : IDamagePopup
{
    public async Task ShowAsync(int amount, CancellationToken ct)
    {
        GD.Print($"[Popup] Damage: {amount}");
        await Task.Delay(200, ct); // simulate popup animation
        GD.Print($"[Popup] Done");
    }
}

public sealed class TestDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.True;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
        => 42;
}

public sealed class TestDamageRegistry : IDamageCalculatorRegistry
{
    private readonly IDamageCalculator _calc = new TestDamageCalculator();
    public IDamageCalculator Get(DamageType type) => _calc;
}
