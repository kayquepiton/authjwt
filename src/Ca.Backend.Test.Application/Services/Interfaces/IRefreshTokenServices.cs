using Ca.Backend.Test.Application.Models.Response;

namespace Ca.Backend.Test.Application.Services.Interfaces;

public interface IRefreshTokenServices
{
    Task<AuthenticateResponse> RefreshTokenAsync(string refreshToken);
}

