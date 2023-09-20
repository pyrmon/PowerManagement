using Newtonsoft.Json;
using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Handler;
using PowerManagement.Lambda.Core.Models;
using Renci.SshNet;

namespace PowerManagement.Lambda.Core.Tests.Handler;

public class RequestHandlerTests
{
    private readonly Fixture _fixture;
    private readonly ILogger _logger;
    private readonly ISshSender _sshSender;
    private ActionElement _actionElement = null!;
    private RequestHandler _requestHandler = null!;

    public RequestHandlerTests()
    {
        _logger = Substitute.For<ILogger>();
        _sshSender = Substitute.For<ISshSender>();
        _fixture = new Fixture();
    }

    [Fact]
    public void Handle_WhenValidRequestWasSend_ShouldSucceed()
    {
        // Arrange
        var actionPlan = _fixture.Create<ActionPlan>();
        actionPlan.Actions.FirstOrDefault()!.SsmKeyPath = "/tmp/hello";
        var parameterValue = JsonConvert.SerializeObject(actionPlan);
        _actionElement = actionPlan.Actions.FirstOrDefault()!;
        _requestHandler = new RequestHandler(parameterValue, _logger, _sshSender);
        var request = new RequestModel
        {
            Request = _actionElement.Action
        };
        var privateKeyContents = File.ReadAllText("HelperFiles/RandomPrivateKey.txt");
        File.WriteAllTextAsync(_actionElement.SsmKeyPath, privateKeyContents);
        var privateKeyFile = new PrivateKeyFile(_actionElement.SsmKeyPath);
        var authMethod =
            new PrivateKeyAuthenticationMethod(_actionElement.Username, privateKeyFile);
        var connection =
            new ConnectionInfo(_actionElement.Hostname, _actionElement.Port, _actionElement.Username, authMethod);
        _sshSender.GetCertificateBasedConnection(_actionElement.Hostname, _actionElement.Port, _actionElement.Username,
            _actionElement.SsmKeyPath).Returns(connection);
        _sshSender.RunSshRequest(connection, _actionElement.Command).Returns(true);
        var expectedMessage = _actionElement.Message;

        // Act
        var response = _requestHandler.Handle(request);

        // Assert
        Assert.Equal(expectedMessage, response.Result);
        _sshSender.Received(1).GetCertificateBasedConnection(_actionElement.Hostname, _actionElement.Port,
            _actionElement.Username, _actionElement.SsmKeyPath);
        _sshSender.Received(1).RunSshRequest(connection, _actionElement.Command);
    }

    [Fact]
    public void Handle_WhenActionPlanReceivedIsInvalid_ShouldThrowJsonException()
    {
        // Arrange
        var actionPlan = "{\"error\":\"invalid_object\"}";
        var parameterValue = JsonConvert.SerializeObject(actionPlan);
        _requestHandler = new RequestHandler(parameterValue, _logger, _sshSender);
        var request = new RequestModel
        {
            Request = "invalid"
        };

        // Act & Assert
        Assert.ThrowsAsync<JsonException>(() => _requestHandler.Handle(request));
        _sshSender.Received(0).GetCertificateBasedConnection(Arg.Any<string>(), Arg.Any<int>(),
            Arg.Any<string>(), Arg.Any<string>());
        _sshSender.Received(0).RunSshRequest(Arg.Any<ConnectionInfo>(), Arg.Any<string>());
    }

    [Fact]
    public void Handle_WhenActionDoesntMatchAnyActionElement_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var actionPlan = _fixture.Create<ActionPlan>();
        var parameterValue = JsonConvert.SerializeObject(actionPlan);
        _requestHandler = new RequestHandler(parameterValue, _logger, _sshSender);
        var request = new RequestModel
        {
            Request = "invalid"
        };

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => _requestHandler.Handle(request));
        _sshSender.Received(0).GetCertificateBasedConnection(Arg.Any<string>(), Arg.Any<int>(),
            Arg.Any<string>(), Arg.Any<string>());
        _sshSender.Received(0).RunSshRequest(Arg.Any<ConnectionInfo>(), Arg.Any<string>());
    }
}