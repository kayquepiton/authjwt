using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;

namespace Ca.Backend.Test.Application.Services.Interfaces;

public interface IRoleServices
{
    Task<RoleResponse> CreateAsync(RoleRequest roleRequest);
    Task<RoleResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<RoleResponse>> GetAllAsync();
    Task<RoleResponse> UpdateAsync(Guid id, RoleRequest roleRequest);
    Task DeleteByIdAsync(Guid id);
}
