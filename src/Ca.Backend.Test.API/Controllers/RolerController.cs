using Ca.Backend.Test.API.Models.Response.Api;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleServices _roleServices;

    public RoleController(IRoleServices roleServices)
    {
        _roleServices = roleServices;
    }

    /// <summary> Cria uma nova role </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/role
    ///     {
    ///        "name": "Admin",
    ///        "description": "Administrador do sistema"
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">Dados da role</param>
    /// <returns>Retorna a role criada</returns>
    /// <response code="200">OK - Role criada com sucesso</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpPost]
    [ProducesResponseType(typeof(GenericHttpResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] RoleRequest request)
    {
        var response = await _roleServices.CreateAsync(request);
        return Ok(new GenericHttpResponse<RoleResponse>
        {
            Data = response
        });
    }

    /// <summary> Obtém uma role pelo ID </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     GET /api/role/{id}
    ///     
    /// </remarks>
    /// <param name="id">ID da role</param>
    /// <returns>Retorna a role correspondente</returns>
    /// <response code="200">OK - Role encontrada</response>
    /// <response code="404">Not Found - Role não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleByIdAsync(Guid id)
    {
        var response = await _roleServices.GetByIdAsync(id);
        return Ok(new GenericHttpResponse<RoleResponse>
        {
            Data = response
        });
    }

    /// <summary> Obtém todas as roles </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     GET /api/role
    ///     
    /// </remarks>
    /// <returns>Retorna uma lista de roles</returns>
    /// <response code="200">OK - Roles encontradas</response>
    /// <response code="400">Bad Request - Requisição do Cliente é Inválida</response>
    [HttpGet]
    [ProducesResponseType(typeof(GenericHttpResponse<IEnumerable<RoleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        var response = await _roleServices.GetAllAsync();
        return Ok(new GenericHttpResponse<IEnumerable<RoleResponse>>
        {
            Data = response
        });
    }

    /// <summary> Atualiza uma role pelo ID </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/role/{id}
    ///     {
    ///        "name": "Admin",
    ///        "description": "Administrador do sistema"
    ///     }
    ///     
    /// </remarks>
    /// <param name="id">ID da role</param>
    /// <param name="request">Dados da role</param>
    /// <returns>Retorna a role atualizada</returns>
    /// <response code="200">OK - Role atualizada com sucesso</response>
    /// <response code="404">Not Found - Role não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoleAsync(Guid id, [FromBody] RoleRequest request)
    {
        var response = await _roleServices.UpdateAsync(id, request);
        return Ok(new GenericHttpResponse<RoleResponse>
        {
            Data = response
        });
    }

    /// <summary> Deleta uma role pelo ID </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     DELETE /api/role/{id}
    ///     
    /// </remarks>
    /// <param name="id">ID da role</param>
    /// <response code="204">No Content - Role deletada com sucesso</response>
    /// <response code="404">Not Found - Role não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoleAsync(Guid id)
    {
        await _roleServices.DeleteByIdAsync(id);
        return NoContent();
    }
}
