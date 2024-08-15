using Ca.Backend.Test.API.Models.Response.Api;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;

    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    /// <summary> Cria um novo usuário </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/user
    ///     {
    ///        "name": "John Doe",
    ///        "username": "johndoe",
    ///        "password": "P@ssw0rd!",
    ///        "roleIds": ["guid1", "guid2"]
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">Dados do usuário</param>
    /// <returns>Retorna o usuário criado</returns>
    /// <response code="200">OK - Usuário criado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpPost]
    [ProducesResponseType(typeof(GenericHttpResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequest request)
    {
        var response = await _userServices.CreateAsync(request);
        return Ok(new GenericHttpResponse<UserResponse>
        {
            Data = response
        });
    }

    /// <summary> Obtém um usuário pelo ID </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     GET /api/user/{id}
    ///     
    /// </remarks>
    /// <param name="id">ID do usuário</param>
    /// <returns>Retorna o usuário correspondente</returns>
    /// <response code="200">OK - Usuário encontrado</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserByIdAsync(Guid id)
    {
        var response = await _userServices.GetByIdAsync(id);
        return Ok(new GenericHttpResponse<UserResponse>
        {
            Data = response
        });
    }

    /// <summary> Obtém todos os usuários </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     GET /api/user
    ///     
    /// </remarks>
    /// <returns>Retorna uma lista de usuários</returns>
    /// <response code="200">OK - Usuários encontrados</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpGet]
    [ProducesResponseType(typeof(GenericHttpResponse<IEnumerable<UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var response = await _userServices.GetAllAsync();
        return Ok(new GenericHttpResponse<IEnumerable<UserResponse>>
        {
            Data = response
        });
    }

    /// <summary> Atualiza um usuário pelo ID </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/user/{id}
    ///     {
    ///        "name": "John Doe",
    ///        "username": "johndoe",
    ///        "password": "newP@ssw0rd!",
    ///        "roleIds": ["guid1", "guid2"]
    ///     }
    ///     
    /// </remarks>
    /// <param name="id">ID do usuário</param>
    /// <param name="request">Dados do usuário</param>
    /// <returns>Retorna o usuário atualizado</returns>
    /// <response code="200">OK - Usuário atualizado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UserRequest request)
    {
        var response = await _userServices.UpdateAsync(id, request);
        return Ok(new GenericHttpResponse<UserResponse>
        {
            Data = response
        });
    }

    /// <summary> Deleta um usuário pelo ID </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     DELETE /api/user/{id}
    ///     
    /// </remarks>
    /// <param name="id">ID do usuário</param>
    /// <response code="204">No Content - Usuário deletado com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUserAsync(Guid id)
    {
        await _userServices.DeleteByIdAsync(id);
        return NoContent();
    }
}
