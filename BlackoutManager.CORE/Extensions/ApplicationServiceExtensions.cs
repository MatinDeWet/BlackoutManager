using BlackoutManager.DATA.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
}
