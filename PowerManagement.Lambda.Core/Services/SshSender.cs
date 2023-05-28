using PowerManagement.Lambda.Core.Contracts;
using Renci.SshNet;

namespace PowerManagement.Lambda.Core.Services;

public class SshSender : ISshSender
{
    private readonly IKeyCreator _keyCreator;
    private readonly ILogger _logger;
    private readonly ISshClientFactory _sshClientFactory;

    public SshSender(ILogger logger, ISshClientFactory sshClientFactory, IKeyCreator keyCreator)
    {
        _logger = logger;
        _sshClientFactory = sshClientFactory;
        _keyCreator = keyCreator;
    }

    public ConnectionInfo GetCertificateBasedConnection(string actionElementHostname, int actionElementPort,
        string actionElementUsername, string actionElementSsmKeyPath)
    {
        var privateKeyPath = _keyCreator.CreatePrivateKey(actionElementSsmKeyPath);
        using var stream = new FileStream(privateKeyPath.Result, FileMode.Open, FileAccess.Read);
        var authMethod = new PrivateKeyAuthenticationMethod(actionElementUsername, new PrivateKeyFile(stream));
        var connection = new ConnectionInfo(actionElementHostname, actionElementPort, actionElementUsername,
            authMethod);
        _logger.LogInfo("Certification based connection created successfully");
        return connection;
    }

    public bool RunSshRequest(ConnectionInfo connection, string actionElementCommand)
    {
        var sshClient = _sshClientFactory.CreateSshClient(connection);
        try
        {
            sshClient.Connect();
            var sshCommand = sshClient.CreateCommand(actionElementCommand);
            sshCommand.Execute();
            return sshCommand.ExitStatus == 0;
        }
        catch (Exception e)
        {
            _logger.LogError($"SSH Client didn't succeed: {e.Message}");
            throw;
        }
    }
}