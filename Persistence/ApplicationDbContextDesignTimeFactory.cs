using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence
{
    public class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            string conn = "Server=mssql.fhict.local;Database=dbi451244_movionsdb;User Id=dbi451244_movionsdb;Password=MovionsDBW8Woord;";
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionBuilder
                .UseSqlServer(
                    conn,
                    b => b.MigrationsAssembly("Persistence")
                );

            return new ApplicationDbContext(optionBuilder.Options);
        }
    }
}
