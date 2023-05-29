using Amazon.Lambda.APIGatewayEvents;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Models;

namespace PowerManagement.Lambda.Tests;

public class FunctionTests
{
    private readonly APIGatewayHttpApiV2ProxyRequest _request;
    private readonly IRequestHandler _requestHandler;
    private readonly ILogger _logger;
    private readonly Fixture _fixture;

    public FunctionTests()
    {
        _fixture = new Fixture();
        _request = _fixture.Create<APIGatewayHttpApiV2ProxyRequest>();
        _requestHandler = Substitute.For<IRequestHandler>();
        _logger = Substitute.For<ILogger>();
    }

    // Write a unit test that checks that if the FunctionHandler receives a good RequestModel it returns a 200 response and a good string
    [Fact]
    public void FunctionHandler_WhenGoodRequestIsReceivedAndNoErrorIsThrown_ShouldSucceed()
    {
        // Arrange
        var function = new Function(GetServiceProvider());
        var request = _fixture.Create<RequestModel>();
        _request.Body = JsonConvert.SerializeObject(request);
        var expectedMessage = _fixture.Create<string>();
        _requestHandler.Handle(Arg.Any<RequestModel>()).Returns(expectedMessage);
        var expectedArray = new Dictionary<string, string> { { "message", expectedMessage } };

        // Act
        var response = function.FunctionHandler(_request);
        // Assert
        Assert.Equal(200, response.Result.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedArray), response.Result.Body);
        _requestHandler.Received(1).Handle(Arg.Any<RequestModel>());
    }

    [Fact]
    public void
        FunctionHandler_WhenJsonSerializationExceptionIsThrown_ShouldReturn400Error()
    {
        // Arrange
        var function = new Function(GetServiceProvider());
        var request = _fixture.Create<RequestModel>();
        _request.Body = JsonConvert.SerializeObject(request);
        var errorMessage = _fixture.Create<string>();
        _requestHandler.Handle(Arg.Any<RequestModel>()).Throws(new JsonSerializationException(errorMessage));
        var expectedArray = new Dictionary<string, string>
            { { "message", $"The RequestBody {_request.Body} is an invalid request body" } };

        // Act
        var response = function.FunctionHandler(_request);
        // Assert
        Assert.Equal(400, response.Result.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedArray), response.Result.Body);
        _requestHandler.Received(1).Handle(Arg.Any<RequestModel>());
    }

    [Fact]
    public void FunctionHandler_WhenKeyNotFoundExceptionIsThrown_ShouldReturn404Error()
    {
        // Arrange
        var function = new Function(GetServiceProvider());
        var request = _fixture.Create<RequestModel>();
        _request.Body = JsonConvert.SerializeObject(request);
        var errorMessage = _fixture.Create<string>();
        _requestHandler.Handle(Arg.Any<RequestModel>()).Throws(new KeyNotFoundException(errorMessage));
        var expectedArray = new Dictionary<string, string>
            { { "message", $"The RequestBody {_request.Body} has an invalid key. Error Message: {errorMessage}" } };

        // Act
        var response = function.FunctionHandler(_request);
        // Assert
        Assert.Equal(404, response.Result.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedArray), response.Result.Body);
        _requestHandler.Received(1).Handle(Arg.Any<RequestModel>());
    }

    [Fact]
    public void FunctionHandler_WhenExceptionIsThrown_ShouldReturn500Error()
    {
        // Arrange
        var function = new Function(GetServiceProvider());
        var request = _fixture.Create<RequestModel>();
        _request.Body = JsonConvert.SerializeObject(request);
        var errorMessage = _fixture.Create<string>();
        _requestHandler.Handle(Arg.Any<RequestModel>()).Throws(new Exception(errorMessage));
        var expectedArray = new Dictionary<string, string>
            { { "message", $"{errorMessage}" } };

        // Act
        var response = function.FunctionHandler(_request);
        // Assert
        Assert.Equal(500, response.Result.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedArray), response.Result.Body);
        _requestHandler.Received(1).Handle(Arg.Any<RequestModel>());
    }

    private IServiceCollection GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddTransient(_ => _requestHandler);
        services.AddTransient(_ => _logger);
        return services;
    }
}