using PowerManagement.Lambda.Core.Contracts;
using Renci.SshNet;

namespace PowerManagement.Lambda.Core.Services;

public class SshClientFactory : ISshClientFactory
{
    public SshClient CreateSshClient(ConnectionInfo connectionInfo)
    {
        var sshClient = new SshClient(connectionInfo);
        return sshClient;
    }
}