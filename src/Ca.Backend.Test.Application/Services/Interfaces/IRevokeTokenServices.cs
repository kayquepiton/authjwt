namespace Ca.Backend.Test.Application.Services.Interfaces;

public interface IRevokeTokenServices
{
    Task<bool> RevokeTokenAsync(string refreshToken);
}

