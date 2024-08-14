using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;

namespace Ca.Backend.Test.Infra.Data.Repository;
public interface IUserRepository : IGenericRepository<UserEntity>
{
    Task<UserEntity?> GetUserByUsernameAsync(string username);
    Task<UserEntity?> GetUserByRefreshTokenAsync(string refreshToken);
}

