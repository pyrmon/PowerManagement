using PowerManagement.Lambda.Core.Contracts;
using PowerManagement.Lambda.Core.Models;

namespace PowerManagement.Lambda.Core.Handler;

public class RequestHandler : IRequestHandler
{
    public Task<string> Handle(RequestModel request)
    {
        throw new NotImplementedException();
    }
}