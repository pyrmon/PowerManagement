namespace PowerManagement.Lambda.Core.Contracts;

public interface IKeyCreator
{
    Task<string> CreatePrivateKey(string actionElementSsmKeyPath);
}