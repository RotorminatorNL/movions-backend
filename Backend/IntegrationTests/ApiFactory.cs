using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using PersistenceInterface;
using System.Linq;

namespace IntegrationTests
{
    public class ApiFactory<TEntryPoint> : 
        WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IHostBuilder CreateHostBuilder() =>
            base.CreateHostBuilder().UseEnvironment("Testing");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IApplicationDbContext));

                services.Remove(descriptor);

                services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "movions_testdb"));
            });
        }
    }
}
