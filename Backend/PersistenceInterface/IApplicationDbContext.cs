using Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace PersistenceInterface
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Genre> Genres { get; }
        DbSet<Language> Languages { get; }
        DbSet<Movie> Movies { get; }
        DbSet<CrewMember> CrewRoles { get; }
        DbSet<Person> Persons { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
