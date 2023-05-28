using Amazon.Lambda.APIGatewayEvents;
using AutoFixture;

namespace PowerManagement.Lambda.Tests;

public class FunctionTests
{
    private readonly APIGatewayHttpApiV2ProxyRequest _request;

    public FunctionTests()
    {
        var fixture = new Fixture();
        _request = fixture.Create<APIGatewayHttpApiV2ProxyRequest>();
    }

    [Fact]
    public void TestFunctionHandler()
    {
        //Arrange
        var function = new Function();
        //Act
        var response = function.FunctionHandler(_request, null);
        //Assert
        Assert.Equal("Welcome", response.Result.Body);
        Assert.Equal(200, response.Result.StatusCode);
    }
}