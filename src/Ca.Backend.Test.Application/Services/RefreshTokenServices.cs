using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using System.Security.Claims;
using Ca.Backend.Test.Infra.Data.Repository;

namespace Ca.Backend.Test.Application.Services;

public class RefreshTokenServices : IRefreshTokenServices
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ITokenServices _tokenServices;

    public RefreshTokenServices(IUserRepository userRepository, IMapper mapper, ITokenServices tokenServices)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenServices = tokenServices;
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);

        if (user is null)
            throw new ApplicationException("User not found or refresh token is invalid.");

        if (DateTime.UtcNow >= user.ExpirationRefreshToken)
            throw new ApplicationException("Refresh token has expired. Please authenticate again.");

        var (accessToken, expiresAccessToken) = GenerateAccessToken(user);
        var (refreshToken, expiresRefreshToken) = await GenerateRefreshTokenAndUpdateDatabaseAsync(user);

        var response = new RefreshTokenResponse
        {
            Authenticated = true,
            Created = DateTime.UtcNow,
            AccessToken = accessToken,
            AccessTokenExpiration = expiresAccessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expiresRefreshToken
        };

        return response;
    }

    private (string, DateTime) GenerateAccessToken(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        return _tokenServices.GenerateAccessToken(claims);
    }

    private async Task<(string, DateTime)> GenerateRefreshTokenAndUpdateDatabaseAsync(UserEntity user)
    {
        var (refreshToken, expiresRefreshToken) = _tokenServices.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.ExpirationRefreshToken = expiresRefreshToken;

        await _userRepository.UpdateAsync(user);

        return (refreshToken, expiresRefreshToken);
    }

}
