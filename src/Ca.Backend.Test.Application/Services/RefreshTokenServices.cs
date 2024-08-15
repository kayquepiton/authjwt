using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Infra.Data.Repository;

namespace Ca.Backend.Test.Application.Services;

public class RefreshTokenServices : IRefreshTokenServices
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGeneratorServices _tokenGeneratorServices;

    public RefreshTokenServices(
        IUserRepository userRepository,
        ITokenGeneratorServices tokenGeneratorServices)
    {
        _userRepository = userRepository;
        _tokenGeneratorServices = tokenGeneratorServices;
    }

    public async Task<AuthenticateResponse> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user is null || user.ExpirationRefreshToken <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var accessTokenResult = _tokenGeneratorServices.GenerateJwtToken(user);
        var newRefreshTokenResult = _tokenGeneratorServices.GenerateRefreshToken();

        user.RefreshToken = newRefreshTokenResult.Token;
        user.ExpirationRefreshToken = newRefreshTokenResult.Expiration;
        await _userRepository.UpdateAsync(user);

        return new AuthenticateResponse
        {
            Authenticated = true,
            AccessToken = accessTokenResult.Token,
            AccessTokenExpiration = accessTokenResult.Expiration,
            RefreshToken = newRefreshTokenResult.Token,
            RefreshTokenExpiration = newRefreshTokenResult.Expiration
        };
    }
}

