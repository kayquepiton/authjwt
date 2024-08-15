using Ca.Backend.Test.Domain.Entities;

namespace Ca.Backend.Test.Application.Services.Interfaces;
public interface ITokenGeneratorServices
{
    (string Token, DateTime Expiration) GenerateJwtToken(UserEntity user);
    (string Token, DateTime Expiration) GenerateRefreshToken();
}

