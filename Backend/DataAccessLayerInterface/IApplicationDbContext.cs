using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayerInterface
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; set; }
        DbSet<Genre> Genres { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Movie> Movies { get; set; }
        DbSet<CrewRole> CrewRoles { get; set; }
        DbSet<Person> Persons { get; set; }
    }
}
