using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence
{
    public class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            string conn = "Server=localhost;Uid=root;Database=movionsdb;Pwd=;";
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionBuilder
                .UseMySql(
                    conn,
                    ServerVersion.AutoDetect(conn)
                )
                .UseSnakeCaseNamingConvention();

            return new ApplicationDbContext(optionBuilder.Options);
        }
    }
}
