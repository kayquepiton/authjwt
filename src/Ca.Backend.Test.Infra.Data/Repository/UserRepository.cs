using Ca.Backend.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ca.Backend.Test.Infra.Data.Repository;
public class UserRepository : GenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IList<UserEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Roles)
            .ToListAsync();
    }

    public override async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.Roles) 
            .SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserEntity?> GetUserByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Roles) 
            .SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserEntity?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet
            .Include(u => u.Roles) 
            .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
