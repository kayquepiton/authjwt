using Ca.Backend.Test.Application.Models.Request;

namespace Cepedi.Serasa.Cadastro.Domain.Services.Auth;

public interface IRevokeTokenServices
{
    Task<bool> RevokeTokenAsync(RefreshTokenRequest request);
}

