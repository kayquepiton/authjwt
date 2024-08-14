using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Infra.Data.Repository;
using Cepedi.Serasa.Cadastro.Domain.Services.Auth;

namespace Ca.Backend.Test.Application.Services;

public class RevokeTokenServices : IRevokeTokenServices
{
    private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

    public RevokeTokenServices(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<bool> RevokeTokenAsync(RefreshTokenRequest request)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);

        if (user is null)
            throw new ApplicationException($"Invalid refresh token for request: {request.RefreshToken}");
        
        user.RefreshToken = null;
        user.ExpirationRefreshToken = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return true;
    }

}
