using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ProjectName.Core.Master.Category;
using ProjectName.Core.User;
namespace ProjectName.Core;
public static class CoreDependencyInjections
{
    public static IServiceCollection RS_CoreDependencyInjections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
