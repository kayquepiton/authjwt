using System.Diagnostics.CodeAnalysis;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Services;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Application.Validators;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data;
using Ca.Backend.Test.Infra.Data.Repository;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ca.Backend.Test.Infra.IoC;
[ExcludeFromCodeCoverage]
public static class IoCServiceExtension
{
    public static void ConfigureAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDbContext(services, configuration);
        services.AddScoped<IGenericRepository<CustomerEntity>, GenericRepository<CustomerEntity>>();
        services.AddScoped<IGenericRepository<ProductEntity>, GenericRepository<ProductEntity>>();
        services.AddScoped<IGenericRepository<BillingEntity>, BillingRepository>();
        services.AddScoped<IGenericRepository<BillingLineEntity>, GenericRepository<BillingLineEntity>>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGenericRepository<RoleEntity>, GenericRepository<RoleEntity>>();

        services.AddScoped<ICustomerServices, CustomerServices>();
        services.AddScoped<IProductServices, ProductServices>();
        services.AddScoped<IBillingServices, BillingServices>();
        services.AddScoped<IUserServices, UserServices>();
        services.AddScoped<IRoleServices, RoleServices>();
        services.AddScoped<IPasswordHasherServices, PasswordHasherServices>();
        services.AddScoped<IAuthenticateServices, AuthenticateServices>();
        services.AddScoped<IRefreshTokenServices, RefreshTokenServices>();
        services.AddScoped<IRevokeTokenServices, RevokeTokenServices>();
        services.AddScoped<ITokenGeneratorServices, TokenGeneratorServices>();
        
        services.AddScoped<IValidator<ProductRequest>, ProductRequestValidator>();
        services.AddScoped<IValidator<CustomerRequest>, CustomerRequestValidator>();
        services.AddScoped<IValidator<BillingRequest>, BillingRequestValidator>();
        services.AddScoped<IValidator<UserRequest>, UserRequestValidator>();
        services.AddScoped<IValidator<RoleRequest>, RoleRequestValidator>();
        services.AddScoped<IValidator<AuthenticateRequest>, AuthenticateRequestValidator>();
        services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();

    }

    private static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
    }
}
