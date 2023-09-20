using System.Diagnostics.CodeAnalysis;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Handler;
using PowerManagement.Lambda.Core.Services;

namespace PowerManagement.Lambda;

[ExcludeFromCodeCoverage]
public class Bootstrapper
{
    protected Bootstrapper()
    {
    }

    public static IServiceCollection Container => ConfigureServices();

    private static IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        AddAwsServices(services);
        AddOwnServices(services);

        return services;
    }

    private static void AddAwsServices(IServiceCollection services)
    {
        var configuration = BuildConfiguration();

        services.AddSingleton(configuration);
        services.AddSingleton<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>();

        var ssmClient = services.BuildServiceProvider().GetRequiredService<IAmazonSimpleSystemsManagement>();
        var parameterValue = RetrieveActionPlanParameter(ssmClient);

        services.AddSingleton(parameterValue);
    }

    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder();
        return builder.Build();
    }

    private static string RetrieveActionPlanParameter(IAmazonSimpleSystemsManagement ssmClient)
    {
        var response = ssmClient.GetParameterAsync(new GetParameterRequest
        {
            Name = "/ssh/actionPlan",
            WithDecryption = true
        }).GetAwaiter().GetResult();
        return response.Parameter.Value;
    }

    private static void AddOwnServices(IServiceCollection services)
    {
        services.AddTransient<ILogger, Logger>();
        services.AddTransient<IRequestHandler, RequestHandler>();
        services.AddTransient<ISshSender, SshSender>();
        services.AddTransient<ISshClientFactory, SshClientFactory>();
        services.AddTransient<IKeyCreator, KeyCreator>();
    }
}