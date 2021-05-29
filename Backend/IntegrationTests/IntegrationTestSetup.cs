using API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersistenceInterface;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationTestSetup : IClassFixture<ApiFactory<Startup>>, IDisposable
    {
        private readonly ApiFactory<Startup> _apiFactory;
        private readonly IServiceScope _serviceScope;
        private readonly HttpClient _httpClient;

        public IntegrationTestSetup(ApiFactory<Startup> factory)
        {
            _apiFactory = factory;
            _serviceScope = _apiFactory.Services.GetService<IServiceScopeFactory>()?.CreateScope();
            _httpClient = _apiFactory.CreateClient();
        }

        protected IApplicationDbContext GetDbContext()
        {
            return _serviceScope.ServiceProvider.GetService<IApplicationDbContext>();
        }

        protected async Task DeleteDbContent()
        {
            if (_serviceScope.ServiceProvider.GetService<IApplicationDbContext>() is DbContext dbContext)
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }

        protected HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
