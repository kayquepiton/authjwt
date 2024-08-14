using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using FluentValidation;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Ca.Backend.Test.Infra.Data.Repository;

namespace Ca.Backend.Test.Application.Services;

public class AuthenticateUserServices : IAuthenticateUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ITokenServices _tokenService;

    public AuthenticateUserServices(IUserRepository userRepository, IMapper mapper,
                                    ITokenServices tokenService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest request)
    {

        var user = await _userRepository.GetUserByUsernameAsync(request.Username);

        if(user is null)
            throw new ApplicationException($"User with username '{request.Username}' was not found.");
        
        var EncryptedPassword = EncryptPassword(request.Password);
        
        if (!EncryptedPassword.Equals(user.Password))
            throw new ApplicationException("The provided password is incorrect.");

        var (accessToken, expiresAccessToken) = GenerateAccessToken(user);
        var (refreshToken, expiresRefreshToken) = await GenerateRefreshTokenAndUpdateDatabaseAsync(user);

        var response = new AuthenticateUserResponse
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
    

    private async Task<(string, DateTime)> GenerateRefreshTokenAndUpdateDatabaseAsync(UserEntity user)
    {
        var (refreshToken, expiresRefreshToken) = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.ExpirationRefreshToken = expiresRefreshToken;
        
        await _userRepository.UpdateAsync(user);

        return (refreshToken, expiresRefreshToken);
    }

    private (string, DateTime) GenerateAccessToken(UserEntity user)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, "ADM")
        };
        
        return _tokenService.GenerateAccessToken(claims);
    }

    private string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        string hashedPassword = Convert.ToBase64String(hashedBytes);

        return hashedPassword;
    }
}
