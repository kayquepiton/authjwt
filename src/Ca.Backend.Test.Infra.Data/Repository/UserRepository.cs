using Ca.Backend.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ca.Backend.Test.Infra.Data.Repository;
public class UserRepository : GenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserEntity?> GetUserByUsernameAsync(string username)
    {
        return await _dbSet.SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserEntity?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

}

