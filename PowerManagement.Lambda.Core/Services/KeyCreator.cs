using PowerManagement.Lambda.Core.Contracts;

namespace PowerManagement.Lambda.Core.Services;

public class KeyCreator : IKeyCreator
{
    public async Task<string> CreatePrivateKey(string actionElementSsmKeyPath)
    {
        throw new NotImplementedException();
    }
}