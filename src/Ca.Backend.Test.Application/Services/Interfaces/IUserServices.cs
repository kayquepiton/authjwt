using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;

namespace Ca.Backend.Test.Application.Services.Interfaces;

public interface IUserServices
{
    Task<UserResponse> CreateAsync(UserRequest userRequest);
    Task<UserResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> UpdateAsync(Guid id, UserRequest userRequest);
    Task DeleteByIdAsync(Guid id);
}

