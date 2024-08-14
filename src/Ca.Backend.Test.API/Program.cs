using System.Reflection;
using System.Text;
using Ca.Backend.Test.API.Middlewares;
using Ca.Backend.Test.Application.Mappings;
using Ca.Backend.Test.Infra.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ca.Backend.Test.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load JWT configuration
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<TokenConfiguration>();

builder.Services.Configure<TokenConfiguration>(builder.Configuration.GetSection("Jwt"));
//builder.Services.AddSingleton(jwtConfig);

// Configure services
ConfigureServices(builder.Services, builder.Configuration, jwtConfig);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration, TokenConfiguration jwtConfig)
{
    // Adds controllers to the services container
    services.AddControllers();

    // Configures API behavior options
    services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true; // Suppresses the automatic model state validation
    });

    // Adds AutoMapper to the container with the specified profile
    services.AddAutoMapper(typeof(MappingProfile));

    // Adds services for API endpoints exploration and Swagger/OpenAPI configuration
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Customer Billing Management API .NET8 (Base)",
            Version = "v1",
            Description = "This project is a REST API developed in .NET 8.0 to manage customer billing.",
            Contact = new OpenApiContact
            {
                Name = "Kayque Almeida Piton",
                Email = "kayquepiton@gmail.com",
                Url = new Uri("https://github.com/kayquepiton")
            }
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Authorization header using the Bearer scheme."
        });;
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
            }
        });

        // Configures XML comments for Swagger
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    // Configure JWT authentication
    var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

    services.AddAuthorization();

    // Configures application dependencies
    services.ConfigureAppDependencies(configuration);

    // Adds HttpClient to the container
    services.AddHttpClient();
}

void ConfigureMiddleware(WebApplication app)
{
    // Enables Swagger in development environment
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = string.Empty; // Sets the Swagger UI at the app's root
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Billing Management API .NET8 (Base)");
        });
    }

    // Adds authentication and authorization middleware
    app.UseAuthentication();
    app.UseAuthorization();

    // Adds custom exception handling middleware
    app.UseMiddleware<ExceptionMiddleware>();

    // Adds routing middleware
    app.UseRouting();

    // Redirects HTTP requests to HTTPS
    app.UseHttpsRedirection();

    // Maps attribute-routed controllers
    app.MapControllers();
}
