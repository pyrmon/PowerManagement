using PowerManagement.Lambda.Core.Models;

namespace PowerManagement.Lambda.Core.Contracts;

public interface IRequestHandler
{
    Task<string> Handle(RequestModel request);
}