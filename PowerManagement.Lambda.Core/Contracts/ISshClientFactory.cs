using Renci.SshNet;

namespace PowerManagement.Lambda.Core.Contracts;

public interface ISshClientFactory
{
    SshClient CreateSshClient(ConnectionInfo connectionInfo);
}