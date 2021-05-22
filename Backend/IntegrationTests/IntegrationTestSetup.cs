using API;
using Microsoft.Extensions.DependencyInjection;
using PersistenceInterface;
using System;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationTestSetup : IClassFixture<ApiFactory<Startup>>, IDisposable
    {
        private readonly ApiFactory<Startup> _factory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _serviceScope;

        public IntegrationTestSetup(ApiFactory<Startup> factory)
        {
            _factory = factory;
            _serviceScopeFactory = factory.Services.GetService<IServiceScopeFactory>();
        }

        protected IApplicationDbContext GetNewDbContext()
        {
            _serviceScope = _serviceScopeFactory?.CreateScope();
            var dbContext = _serviceScope.ServiceProvider.GetService<IApplicationDbContext>();

            return dbContext;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
