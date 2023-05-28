using Renci.SshNet;

namespace PowerManagement.Lambda.Core.Contracts;

public interface ISshSender
{
    ConnectionInfo GetCertificateBasedConnection(string actionElementHostname, int actionElementPort,
        string actionElementUsername, string actionElementSsmKeyPath);

    bool RunSshRequest(ConnectionInfo connection, string actionElementCommand);
}