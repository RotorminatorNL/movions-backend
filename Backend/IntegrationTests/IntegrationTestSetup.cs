using API;
using Microsoft.Extensions.DependencyInjection;
using PersistenceInterface;
using System;
using System.Net.Http;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationTestSetup : IClassFixture<ApiFactory<Startup>>, IDisposable
    {
        private readonly ApiFactory<Startup> _apiFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _serviceScope;

        public IntegrationTestSetup(ApiFactory<Startup> factory)
        {
            _apiFactory = factory;
            _serviceScopeFactory = factory.Services.GetService<IServiceScopeFactory>();
        }

        protected IApplicationDbContext CreateDbContext()
        {
            _serviceScope = _serviceScopeFactory?.CreateScope();
            var dbContext = _serviceScope.ServiceProvider.GetService<IApplicationDbContext>();

            return dbContext;
        }

        protected HttpClient CreateHttpClient()
        {
            var client = _apiFactory.CreateClient();

            return client;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
