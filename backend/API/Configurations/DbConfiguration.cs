using EntityFrameworkCore.UnitOfWork.Extensions;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Configurations
{
    public static class DbConfiguration
    {
        public static void AddDbConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                b => b.MigrationsAssembly("Infraesctructure.Data"));
            }, contextLifetime: ServiceLifetime.Transient);
            services.AddUnitOfWork<DataContext>(ServiceLifetime.Transient);
        }
    }
}
