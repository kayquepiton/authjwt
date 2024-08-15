using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentValidation;

namespace Ca.Backend.Test.Application.Services;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<RoleEntity> _roleRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UserRequest> _userRequestValidator;
    private readonly IPasswordHasherServices _passwordHasherServices;

    public UserServices(
        IUserRepository userRepository,
        IGenericRepository<RoleEntity> roleRepository,
        IMapper mapper,
        IValidator<UserRequest> userRequestValidator,
        IPasswordHasherServices passwordHasherServices)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _userRequestValidator = userRequestValidator;
        _passwordHasherServices = passwordHasherServices;
    }

    public async Task<UserResponse> CreateAsync(UserRequest userRequest)
    {
        var validationResult = await _userRequestValidator.ValidateAsync(userRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await EnsureUserDoesNotExist(userRequest.Username);

        var userEntity = _mapper.Map<UserEntity>(userRequest);
        
        userEntity.PasswordHash = _passwordHasherServices.HashPassword(userRequest.Password);

        userEntity.Roles = await GetRolesByIdsAsync(userRequest.RoleIds);

        userEntity = await _userRepository.CreateAsync(userEntity);

        return _mapper.Map<UserResponse>(userEntity);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity is null)
            throw new ApplicationException($"User with ID {id} not found.");

        return _mapper.Map<UserResponse>(userEntity);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var userEntities = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponse>>(userEntities);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UserRequest userRequest)
    {
        var validationResult = await _userRequestValidator.ValidateAsync(userRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity is null)
            throw new ApplicationException($"User with ID {id} not found.");

        _mapper.Map(userRequest, userEntity);

        if (!string.IsNullOrEmpty(userRequest.Password))
        {
            userEntity.PasswordHash = _passwordHasherServices.HashPassword(userRequest.Password);
        }

        userEntity.Roles = await GetRolesByIdsAsync(userRequest.RoleIds);

        userEntity = await _userRepository.UpdateAsync(userEntity);
        return _mapper.Map<UserResponse>(userEntity);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity is null)
            throw new ApplicationException($"User with ID {id} not found.");

        await _userRepository.DeleteByIdAsync(id);
    }

    private async Task EnsureUserDoesNotExist(string username)
    {
        var existingUser = await _userRepository.GetUserByUsernameAsync(username);
        if (existingUser is not null)
            throw new InvalidOperationException("A user with this username already exists.");
    }

    private async Task<IList<RoleEntity>> GetRolesByIdsAsync(IList<Guid> roleIds)
    {
        var roles = new List<RoleEntity>();

        foreach (var roleId in roleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role is null)
            {
                throw new ApplicationException($"Role with ID {roleId} not found.");
            }
            roles.Add(role);
        }

        return roles;
    }
}
