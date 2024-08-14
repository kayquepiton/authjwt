using System.Security.Claims;

namespace Ca.Backend.Test.Application.Services.Interfaces;
public interface ITokenServices
{
    (string token, DateTime expires) GenerateAccessToken(IEnumerable<Claim> claims);
    (string refreshToken, DateTime expires) GenerateRefreshToken();
}

