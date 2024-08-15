using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;

namespace Ca.Backend.Test.Application.Services.Interfaces;

public interface IAuthenticateServices
{
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest authRequest);
}

