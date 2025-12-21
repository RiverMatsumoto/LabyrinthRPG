using Microsoft.Extensions.DependencyInjection;
using Godot;

public partial class GameServicesBootstrap : Node
{
    public override void _EnterTree()
    {
        GameServices.Build();
    }
}

public static class DI
{
    public static T Get<T>() where T : notnull =>
        GameServices.Provider.GetRequiredService<T>();
}
