using Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayerInterface
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Genre> Genres { get; }
        DbSet<Language> Languages { get; }
        DbSet<Movie> Movies { get; }
        DbSet<CrewRole> CrewRoles { get; }
        DbSet<Person> Persons { get; }

        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
