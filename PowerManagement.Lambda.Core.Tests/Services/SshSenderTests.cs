using AutoFixture.AutoNSubstitute;
using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Models;
using PowerManagement.Lambda.Core.Services;
using FluentAssertions;


namespace PowerManagement.Lambda.Core.Tests.Services;

public class SshSenderTests
{
    private readonly IKeyCreator _keyCreator;
    private readonly ILogger _logger;
    private readonly ISshClientFactory _sshClientFactory;

    public SshSenderTests()
    {
        _logger = Substitute.For<ILogger>();
        _sshClientFactory = Substitute.For<ISshClientFactory>();
        _keyCreator = Substitute.For<IKeyCreator>();
    }


    [Fact]
    public void GetCertificateBasedConnection_IfValidPrivateKey_ShouldNotThrow()
    {
        // Arrange
        _keyCreator.CreatePrivateKey("/id/rsa").Returns("HelperFiles/RandomPrivateKey.txt");
        var actionElement = new ActionElement
        {
            Hostname = "example.com",
            Port = 22,
            Username = "Username",
            SsmKeyPath = "/id/rsa"
        };
        var sender = new SshSender(_logger, _sshClientFactory, _keyCreator);

        // Act
        var response = sender.GetCertificateBasedConnection(actionElement.Hostname, actionElement.Port,
            actionElement.Username,
            actionElement.SsmKeyPath
        );

        // Assert
        Assert.NotNull(response);
        _keyCreator.Received(1).CreatePrivateKey("/id/rsa");
    }

    [Fact]
    public void GetCertificateBasedConnection_IfInvalidPrivateKey_ShouldThrow()
    {
        // Arrange
        _keyCreator.CreatePrivateKey("/id/rsa").Returns("HelperFiles/RandomPrivateKey.txt");
        var actionElement = new ActionElement
        {
            Hostname = "example.com",
            Port = 22,
            Username = "Username",
            SsmKeyPath = "/id/rsa"
        };
        var sender = new SshSender(_logger, _sshClientFactory, _keyCreator);

        // Act & Assert
        Action action = () => sender.GetCertificateBasedConnection(
            actionElement.Hostname,
            actionElement.Port,
            actionElement.Username,
            "InvalidKeyPath"
        );

        action.Should().Throw<ArgumentException>();
        _keyCreator.Received(1).CreatePrivateKey("InvalidKeyPath");
    }
}