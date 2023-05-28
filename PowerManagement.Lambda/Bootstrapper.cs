using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Handler;
using PowerManagement.Lambda.Core.Services;

namespace PowerManagement.Lambda;

public class Bootstrapper
{
    protected Bootstrapper()
    {
    }

    public static IServiceCollection Container => ConfigureServices();

    private static IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        AddOwnServices(services);

        return services;
    }

    private static void AddOwnServices(IServiceCollection services)
    {
        services.AddTransient<ILogger, Logger>();
        services.AddTransient<IRequestHandler, RequestHandler>();
    }
}