using Microsoft.Extensions.DependencyInjection;

public static class DI
{
    public static T Get<T>() where T : notnull =>
        GameServices.Provider.GetRequiredService<T>();
}
