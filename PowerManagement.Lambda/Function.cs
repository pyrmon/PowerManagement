using System;
using System.Collections.Generic;
using Amazon.Lambda.Serialization.SystemTextJson;
using Newtonsoft.Json;
using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Models;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace PowerManagement.Lambda;

public class Function
{
    private readonly IServiceCollection _serviceCollection;

    public Function() : this(Bootstrapper.Container)
    {
    }

    public Function(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest apiGatewayProxyRequest)
    {
        var handler = _serviceCollection.BuildServiceProvider();
        var logger = handler.GetService<ILogger>();
        var requestHandler = handler.GetService<IRequestHandler>();
        var responseBody = new Dictionary<string, string>();
        try
        {
            var request = JsonConvert.DeserializeObject<RequestModel>(apiGatewayProxyRequest.Body);
            var response = await requestHandler.Handle(request);
            responseBody.Add("message", response);
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(responseBody)
            };
        }
        catch (JsonSerializationException)
        {
            logger.LogError($"The RequestBody {apiGatewayProxyRequest.Body} is an invalid request body");
            responseBody.Add("message", $"The RequestBody {apiGatewayProxyRequest.Body} is an invalid request body");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = JsonConvert.SerializeObject(responseBody)
            };
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(
                $"The RequestBody {apiGatewayProxyRequest.Body} has an invalid key. Error Message: {e.Message}");
            responseBody.Add("message",
                $"The RequestBody {apiGatewayProxyRequest.Body} has an invalid key. Error Message: {e.Message}");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 404,
                Body = JsonConvert.SerializeObject(responseBody)
            };
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            responseBody.Add("message", e.Message);
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 500,
                Body = JsonConvert.SerializeObject(responseBody)
            };
        }
    }
}