using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository;
using FluentValidation;

namespace Ca.Backend.Test.Application.Services;

public class UserServices : IUserServices
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<UserRequest> _userRequestValidator;

    public UserServices(IUserRepository repository, IMapper mapper, 
                        IValidator<UserRequest> userRequestValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _userRequestValidator = userRequestValidator;
    }

    public async Task<UserResponse> CreateAsync(UserRequest userRequest)
    {
        var validationResult = await _userRequestValidator.ValidateAsync(userRequest);
        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        await EnsureUserDoesNotExist(userRequest.Username);

        string hashedPassword;
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userRequest.Password));
            hashedPassword = Convert.ToBase64String(hashedBytes);
        }
        
        var userEntity = _mapper.Map<UserEntity>(userRequest);
        userEntity.Password = hashedPassword;
        userEntity = await _repository.CreateAsync(userEntity);

        return _mapper.Map<UserResponse>(userEntity);

    }

    private async Task EnsureUserDoesNotExist(string username)
    {
        var existingUser = await _repository.GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this username already exists.");
        }
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var userEntity = await _repository.GetByIdAsync(id);

        if (userEntity is null)
            throw new ApplicationException($"User with ID {id} not found.");

        return _mapper.Map<UserResponse>(userEntity);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var userEntities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponse>>(userEntities);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UserRequest userRequest)
    {
        var validationResult = await _userRequestValidator.ValidateAsync(userRequest);
        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var userEntity = await _repository.GetByIdAsync(id);
        if (userEntity is null)
            throw new ApplicationException($"User with ID {id} not found.");

        _mapper.Map(userRequest, userEntity);
        userEntity = await _repository.UpdateAsync(userEntity);
        return _mapper.Map<UserResponse>(userEntity);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var userEntity = await _repository.GetByIdAsync(id);

        if (userEntity is null)
            throw new ApplicationException($"User with ID {id} not found.");

        await _repository.DeleteByIdAsync(id);
    }
   
}

