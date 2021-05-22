using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using PersistenceInterface;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _isTesting = env.IsEnvironment("Testing");
        }

        public IConfiguration Configuration { get; }

        private readonly bool _isTesting = false;
        private readonly string AllowFrontend = "_allowFrontend";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_isTesting)
            {
                services.AddTransient<Company>();
                services.AddTransient<CrewMember>();
                services.AddTransient<Genre>();
                services.AddTransient<Language>();
                services.AddTransient<Movie>();
                services.AddTransient<Person>();
            }
            else
            {
                services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
                    options => options.UseSqlServer(
                        Configuration.GetConnectionString("MovionsDB"),
                        b => b.MigrationsAssembly("Persistence")
                    )
                );
            }

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowFrontend,
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(AllowFrontend);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
