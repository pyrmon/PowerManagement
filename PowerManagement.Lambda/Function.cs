using PowerManagement.Lambda.Core.Contracts;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PowerManagement.Lambda;

public class Function
{
    private readonly IServiceCollection _serviceCollection;

    public Function() : this(Bootstrapper.Container)
    {
    }

    private Function(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest apiGatewayProxyRequest, ILambdaContext context)
    {
        var handler = _serviceCollection.BuildServiceProvider();
        var logger = handler.GetService<ILogger>();
        logger.LogInfo("Starting");
        return Task.FromResult(new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = "Welcome"
        });
    }
}