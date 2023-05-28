using Newtonsoft.Json;
using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Models;

namespace PowerManagement.Lambda.Core.Handler;

public class RequestHandler : IRequestHandler
{
    private readonly ILogger _logger;
    private readonly string _parameterValue;
    private readonly ISshSender _sshSender;

    public RequestHandler(string parameterValue, ILogger logger, ISshSender sshSender)
    {
        _parameterValue = parameterValue;
        _logger = logger;
        _sshSender = sshSender;
    }

    public Task<string> Handle(RequestModel request)
    {
        try
        {
            var actionPlan = JsonConvert.DeserializeObject<ActionPlan>(_parameterValue);
            var actionElement = actionPlan!.Actions.FirstOrDefault(x => x.Action == request.Request);
            if (actionElement != null)
            {
                _logger.LogInfo($"{actionElement.Action} is being run");
                var connection = _sshSender.GetCertificateBasedConnection(actionElement.Hostname, actionElement.Port,
                    actionElement.Username, actionElement.SsmKeyPath);
                _sshSender.RunSshRequest(connection, actionElement.Command);
            }

            if (actionElement != null) return Task.FromResult(actionElement.Message);
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(JsonSerializationException)) throw;
            _logger.LogError("The ActionPlan uploaded to SSM is invalid");
            throw new JsonException($"The ActionPlan uploaded to SSM is invalid. ErrorMessage: {e.Message}");
        }

        _logger.LogError($"The Action Key {request.Request} was not in the ActionPlan");
        throw new KeyNotFoundException($"The Action Key {request.Request} was not in the ActionPlan");
    }
}