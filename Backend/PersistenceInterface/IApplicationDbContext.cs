using Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace PersistenceInterface
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<CompanyMovie> CompanyMovies { get; }
        DbSet<CrewMember> CrewMembers { get; }
        DbSet<Genre> Genres { get; }
        DbSet<GenreMovie> GenreMovies { get; }
        DbSet<Language> Languages { get; }
        DbSet<Movie> Movies { get; }
        DbSet<Person> Persons { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
