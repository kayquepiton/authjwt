using Ca.Backend.Test.API.Models.Response.Api;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticateServices _authenticateServices;
    private readonly IRefreshTokenServices _refreshTokenServices;
    private readonly IRevokeTokenServices _revokeTokenServices;

    public AuthenticateController(
        IAuthenticateServices authenticateServices,
        IRefreshTokenServices refreshTokenServices,
        IRevokeTokenServices revokeTokenServices)
    {
        _authenticateServices = authenticateServices;
        _refreshTokenServices = refreshTokenServices;
        _revokeTokenServices = revokeTokenServices;
    }

    /// <summary> Autentica um usuário e retorna tokens de acesso </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/authenticate/signin
    ///     {
    ///        "username": "usuario",
    ///        "password": "senha"
    ///     }
    /// </remarks>
    /// <param name="request">Dados de autenticação do usuário</param>
    /// <returns>Retorna os tokens de autenticação</returns>
    /// <response code="200">OK - Autenticação bem-sucedida</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    /// <response code="401">Unauthorized - Credenciais inválidas</response>
    [HttpPost("signin")]
    [ProducesResponseType(typeof(GenericHttpResponse<AuthenticateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignInAsync([FromBody] AuthenticateRequest request)
    {
        var response = await _authenticateServices.AuthenticateAsync(request);
        return Ok(new GenericHttpResponse<AuthenticateResponse>
        {
            Data = response
        });
    }

    /// <summary> Renova o token de acesso usando o token de refresh </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/authenticate/refresh-token
    ///     {
    ///        "refreshToken": "string"
    ///     }
    /// </remarks>
    /// <param name="request">Token de refresh do usuário</param>
    /// <returns>Retorna os novos tokens de autenticação</returns>
    /// <response code="200">OK - Token renovado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    /// <response code="401">Unauthorized - Token de refresh inválido ou expirado</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(GenericHttpResponse<AuthenticateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var response = await _refreshTokenServices.RefreshTokenAsync(request.RefreshToken);
        return Ok(new GenericHttpResponse<AuthenticateResponse>
        {
            Data = response
        });
    }

    /// <summary> Revoga o token de refresh do usuário </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/authenticate/revoke-token
    ///     {
    ///        "refreshToken": "string"
    ///     }
    /// </remarks>
    /// <param name="request">Token de refresh do usuário</param>
    /// <returns>Retorna uma confirmação de revogação</returns>
    /// <response code="200">OK - Token revogado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    /// <response code="401">Unauthorized - Token de refresh inválido ou expirado</response>
    [HttpPost("revoke")]
    [ProducesResponseType(typeof(GenericHttpResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var success = await _revokeTokenServices.RevokeTokenAsync(request.RefreshToken);
        return Ok(new GenericHttpResponse<bool>
        {
            Data = success
        });
    }
}
