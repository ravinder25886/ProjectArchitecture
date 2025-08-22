using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RS.Dapper.Utility.Repositories.DapperExecutor;
using RS.Dapper.Utility.Repositories.DapperRepository;
using RS.Dapper.Utility.Resolver;

namespace RS.Dapper.Utility;

public static class RSDapperUtilityDependencyInjections
{
    public static IServiceCollection RS_DapperUtilityDependencyInjections(this IServiceCollection services, IConfiguration configuration)
    {
        //Start-RS.Dapper.Utility
        
        services.AddScoped<IDatabaseResolver, DatabaseResolver>();
        services.AddScoped<IDapperRepository, DapperRepository>();
        services.AddScoped<IDapperExecutor, DapperExecutor>();
        //End-RS.Dapper.Utility
        return services;
    }
}
