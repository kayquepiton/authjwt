using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Infra.Data.Repository;

namespace Ca.Backend.Test.Application.Services;

public class RevokeTokenServices : IRevokeTokenServices
{
    private readonly IUserRepository _userRepository;

    public RevokeTokenServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user is null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.ExpirationRefreshToken = DateTime.MinValue;

        await _userRepository.UpdateAsync(user);
        return true;
    }
}

