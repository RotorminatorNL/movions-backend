using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using PersistenceInterface;

namespace Mapping
{
    public static class StartupExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, bool isTesting)
        {
            if (!isTesting)
            {
                string conn = "Server=localhost;Uid=root;Database=movionsdb;Pwd=;";
                services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
                    options => options.UseMySql(
                        conn,
                        ServerVersion.AutoDetect(conn),
                        mySqlOptions => mySqlOptions.MigrationsAssembly("Persistence")
                    )
                    .UseSnakeCaseNamingConvention()
                );
            }

            services.AddTransient<Company>();
            services.AddTransient<CrewMember>();
            services.AddTransient<Genre>();
            services.AddTransient<Language>();
            services.AddTransient<Movie>();
            services.AddTransient<Person>();

            return services;
        }
    }
}
