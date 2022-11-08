using BlackoutManager.DATA.EF;
using BlackoutManager.DATA.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlackoutManager.CORE.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationPostGresContext(this IServiceCollection services, IConfiguration _config)
    {
        services.AddDbContext<PostGresContext>(options =>
        {
            options.UseNpgsql(_config.GetConnectionString("PostGresConnectionString"));
        });

        return services;
    }

    public static IServiceCollection AddApplicationAuthentication(this IServiceCollection services, IConfiguration _config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _config["JWTSettings:Issuer"],
                ValidAudience = _config["JWTSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:Key"]))
            };
        });

        return services;
    }
    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration _config)
    {
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(_config["JWTSettings:Issuer"])
            .AddEntityFrameworkStores<PostGresContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
