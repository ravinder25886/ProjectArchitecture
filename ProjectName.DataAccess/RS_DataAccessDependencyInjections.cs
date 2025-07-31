using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ProjectName.DataAccess.Master.Category;
using ProjectName.DataAccess.User;

namespace ProjectName.DataAccess;
public static class DataAccessDependencyInjections
{
    public static IServiceCollection RS_DataAccessDependencyInjections(this IServiceCollection services, IConfiguration configuration)
    {

        
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
