using Ca.Backend.Test.API.Models.Response.Api;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Cepedi.Serasa.Cadastro.Domain.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticateUserServices _authenticateUserServices;
    private readonly IRefreshTokenServices _refreshTokenServices;
    private readonly IRevokeTokenServices _revokeTokenServices; 

    public AuthController(IAuthenticateUserServices authenticateUserServices, IRefreshTokenServices refreshTokenServices, IRevokeTokenServices revokeTokenServices)
    {
        _authenticateUserServices = authenticateUserServices;
        _refreshTokenServices = refreshTokenServices;
        _revokeTokenServices = revokeTokenServices; 
    }


    /// <summary> Autentica um usuário e retorna tokens </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/authenticate
    ///     {
    ///        "username": "usuario",
    ///        "password": "senha"
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">Dados de autenticação do usuário</param>
    /// <returns>Retorna um response com os tokens de autenticação</returns>
    /// <response code="200">OK - Autenticação bem-sucedida</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpPost("signin")]
    [ProducesResponseType(typeof(GenericHttpResponse<AuthenticateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProductAsync([FromBody] AuthenticateUserRequest request)
    {
        var response = await _authenticateUserServices.AuthenticateAsync(request);
        return Ok(new GenericHttpResponse<AuthenticateUserResponse>
        {
            Data = response
        });
    }


    /// <summary> Atualiza o token de acesso usando o refresh token </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/auth/refresh
    ///     {
    ///        "refreshToken": "token"
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">Dados do refresh token</param>
    /// <returns>Retorna a nova resposta de token</returns>
    /// <response code="200">OK - Token atualizado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(GenericHttpResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var response = await _refreshTokenServices.RefreshTokenAsync(request);
        return Ok(new GenericHttpResponse<RefreshTokenResponse>
        {
            Data = response
        });
    }


    /// <summary> Revoga um refresh token </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/auth/revoke
    ///     {
    ///        "refreshToken": "token"
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">Dados do refresh token a ser revogado</param>
    /// <returns>Retorna um status indicando se a revogação foi bem-sucedida</returns>
    /// <response code="200">OK - Token revogado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var user = await _revokeTokenServices.RevokeTokenAsync(request);
        return Ok(); 
    }
}

