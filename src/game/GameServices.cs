using Microsoft.Extensions.DependencyInjection;

public static class GameServices
{
    public static ServiceProvider Provider { get; private set; } = null;

    public static void Build()
    {
        var services = new ServiceCollection();

        // save systems
        services.AddSingleton<ISaveStore, FileSaveStore>();
        services.AddSingleton<Settings>();

        // battle system
        services.AddSingleton<IActionLibrary, ActionLibrary>();

        // damage calculators
        services.AddSingleton<IDamageCalculator, CutDamageCalculator>();
        services.AddSingleton<IDamageCalculator, BashDamageCalculator>();
        services.AddSingleton<IDamageCalculator, StabDamageCalculator>();
        services.AddSingleton<IDamageCalculator, FireDamageCalculator>();
        services.AddSingleton<IDamageCalculator, IceDamageCalculator>();
        services.AddSingleton<IDamageCalculator, LightningDamageCalculator>();

        services.AddSingleton<IActionLibrary, ActionLibrary>();

        services.AddSingleton<IDamageCalculatorRegistry, DamageCalculatorRegistry>();

        Provider = services.BuildServiceProvider();
    }
}
