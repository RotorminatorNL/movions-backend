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
                string conn = "Server=mssql.fhict.local;Database=dbi451244_movionsdb;User Id=dbi451244_movionsdb;Password=MovionsDBW8Woord;";
                services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
                    options => options.UseSqlServer(
                        conn,
                        b => b.MigrationsAssembly("Persistence")
                    )
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
