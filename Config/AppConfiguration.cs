using System.Text;
using System.Text.Json.Serialization;
using MemoryBox_API.Models;
using MemoryBox_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MemoryBox_API.Config;

public class AppConfiguration(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureAuthServices(services);
        ConfigureDatabaseServices(services);
        ConfigureJsonServices(services);
        ConfigureR2Services(services);
    }
    
    private void ConfigureAuthServices(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtKey = configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("Jwt:Key is not configured");
                }
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
    }
    
    private void ConfigureDatabaseServices(IServiceCollection services)
    {
        // DbContext Service, 依赖注入到 EF Core: DatabaseContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DatabaseContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }
    
    private static void ConfigureJsonServices(IServiceCollection services)
    {
        // Prevent reference loop in JSON serialization.
        // Don't use =ReferenceHandler.Preserve, it will add $id and $values to the JSON.
        services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    }
    
    private static void ConfigureR2Services(IServiceCollection services)
    {
        services.AddSingleton<R2Service>();
    }
}