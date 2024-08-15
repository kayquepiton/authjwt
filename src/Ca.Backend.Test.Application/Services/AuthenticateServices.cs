using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Infra.Data.Repository;

namespace Ca.Backend.Test.Application.Services;

public class AuthenticateServices : IAuthenticateServices
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherServices _passwordHasherServices;
    private readonly ITokenGeneratorServices _tokenGeneratorServices;

    public AuthenticateServices(
        IUserRepository userRepository,
        IPasswordHasherServices passwordHasherServices,
        ITokenGeneratorServices tokenGeneratorServices)
    {
        _userRepository = userRepository;
        _passwordHasherServices = passwordHasherServices;
        _tokenGeneratorServices = tokenGeneratorServices;
    }

    public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest authRequest)
    {
        var user = await _userRepository.GetUserByUsernameAsync(authRequest.Username);

        if (user is null || !_passwordHasherServices.VerifyPassword(authRequest.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var accessTokenResult = _tokenGeneratorServices.GenerateJwtToken(user);
        var refreshTokenResult = _tokenGeneratorServices.GenerateRefreshToken();

        user.RefreshToken = refreshTokenResult.Token;
        user.ExpirationRefreshToken = refreshTokenResult.Expiration;
        await _userRepository.UpdateAsync(user);

        return new AuthenticateResponse
        {
            Authenticated = true,
            Created = DateTime.UtcNow,
            AccessToken = accessTokenResult.Token,
            AccessTokenExpiration = accessTokenResult.Expiration,
            RefreshToken = refreshTokenResult.Token,
            RefreshTokenExpiration = refreshTokenResult.Expiration
        };
    }
}

