using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ca.Backend.Test.Application.Configuration;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ca.Backend.Test.Application.Services;
public class TokenServices : ITokenServices
{
    private readonly TokenConfiguration _configuration;
    private readonly ConcurrentDictionary<string, DateTime> _revokedRefreshTokens = new ConcurrentDictionary<string, DateTime>();

    public TokenServices(IOptions<TokenConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public (string, DateTime) GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        
        var expiresAccesstoken = DateTime.Now.AddMinutes(_configuration.ExpirationInMinutesAccessToken);
        
        var options = new JwtSecurityToken(
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            claims: claims,
            expires: expiresAccesstoken,
            signingCredentials: signinCredentials
        );
        string tokenString = new JwtSecurityTokenHandler().WriteToken(options);
        return (tokenString, expiresAccesstoken);
    }

    public (string, DateTime) GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        var expiresRefreshToken = DateTime.Now.AddMinutes(_configuration.ExpirationInMinutesRefreshToken);

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return (Convert.ToBase64String(randomNumber), expiresRefreshToken);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false // Permitir tokens expirados
        };

        SecurityToken? securityToken = null;
        ClaimsPrincipal? principal = null;

        // Tente validar o token
        tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        // Verifique se o token é um JwtSecurityToken e se o algoritmo de assinatura é HmacSha256
        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        // Obtenha o principal do token
        principal = new ClaimsPrincipal(tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken));

        return principal;
    }

}
