using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentValidation;

namespace Ca.Backend.Test.Application.Services;

public class RoleServices : IRoleServices
{
    private readonly IGenericRepository<RoleEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<RoleRequest> _roleRequestValidator;

    public RoleServices(
        IGenericRepository<RoleEntity> repository,
        IMapper mapper,
        IValidator<RoleRequest> roleRequestValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _roleRequestValidator = roleRequestValidator;
    }

    public async Task<RoleResponse> CreateAsync(RoleRequest roleRequest)
    {
        var validationResult = await _roleRequestValidator.ValidateAsync(roleRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var roleEntity = _mapper.Map<RoleEntity>(roleRequest);
        roleEntity = await _repository.CreateAsync(roleEntity);
        return _mapper.Map<RoleResponse>(roleEntity);
    }

    public async Task<RoleResponse> GetByIdAsync(Guid id)
    {
        var roleEntity = await _repository.GetByIdAsync(id);

        if (roleEntity is null)
            throw new ApplicationException($"Role with ID {id} not found.");

        return _mapper.Map<RoleResponse>(roleEntity);
    }

    public async Task<IEnumerable<RoleResponse>> GetAllAsync()
    {
        var roleEntities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleResponse>>(roleEntities);
    }

    public async Task<RoleResponse> UpdateAsync(Guid id, RoleRequest roleRequest)
    {
        var validationResult = await _roleRequestValidator.ValidateAsync(roleRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var roleEntity = await _repository.GetByIdAsync(id);
        if (roleEntity is null)
            throw new ApplicationException($"Role with ID {id} not found.");

        _mapper.Map(roleRequest, roleEntity);
        roleEntity = await _repository.UpdateAsync(roleEntity);
        return _mapper.Map<RoleResponse>(roleEntity);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var roleEntity = await _repository.GetByIdAsync(id);

        if (roleEntity is null)
            throw new ApplicationException($"Role with ID {id} not found.");

        await _repository.DeleteByIdAsync(id);
    }
}
