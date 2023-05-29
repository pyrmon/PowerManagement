using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using PowerManagement.Lambda.Core.Contracts;

namespace PowerManagement.Lambda.Core.Services;

public class KeyCreator : IKeyCreator
{
    private readonly ILogger _logger;
    private string _privateKeyPath = "";

    public KeyCreator(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<string> CreatePrivateKey(string actionElementSsmKeyPath)
    {
        try
        {
            var privateKeyString = GetSsmParameter(true, actionElementSsmKeyPath);
            _privateKeyPath = "/tmp/id_rsa";
            await File.WriteAllTextAsync(_privateKeyPath, privateKeyString.Result);
            _logger.LogInfo("Private Key was created");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception during Private Key Creation: {ex.Message}");
        }

        return _privateKeyPath;
    }

    private async Task<string> GetSsmParameter(bool encryption, string path)
    {
        var region = RegionEndpoint.EUCentral1;
        var value = string.Empty;
        var request = new GetParameterRequest
        {
            WithDecryption = encryption,
            Name = path
        };

        using var client = new AmazonSimpleSystemsManagementClient(region);
        try
        {
            var response = await client.GetParameterAsync(request);
            value = response.Parameter.Value;
            _logger.LogInfo($"Parameter {request.Name} has received its value");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred: {ex.Message}");
        }

        if (value != null) return value;
        throw new ArgumentNullException(value);
    }
}