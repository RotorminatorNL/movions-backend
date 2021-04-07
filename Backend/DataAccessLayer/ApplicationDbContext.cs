using DataAccessLayerInterface;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CrewRole> CrewRoles { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieCompany> MovieCompanies { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<MovieLanguage> MovieLanguages { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieCompany>()
                .HasKey(mc => new { mc.MovieID, mc.CompanyID });
            modelBuilder.Entity<MovieCompany>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.Companies)
                .HasForeignKey(mc => mc.MovieID);
            modelBuilder.Entity<MovieCompany>()
                .HasOne(mc => mc.Company)
                .WithMany(c => c.Movies)
                .HasForeignKey(mc => mc.CompanyID);

            modelBuilder.Entity<MovieGenre>()
                .HasKey(mc => new { mc.MovieID, mc.GenreID });
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.Genres)
                .HasForeignKey(mc => mc.MovieID);
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mc => mc.Genre)
                .WithMany(c => c.Movies)
                .HasForeignKey(mc => mc.GenreID);

            modelBuilder.Entity<MovieLanguage>()
                .HasKey(mc => new { mc.MovieID, mc.LanguageID });
            modelBuilder.Entity<MovieLanguage>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.Languages)
                .HasForeignKey(mc => mc.MovieID);
            modelBuilder.Entity<MovieLanguage>()
                .HasOne(mc => mc.Language)
                .WithMany(c => c.Movies)
                .HasForeignKey(mc => mc.LanguageID);
        }
    }
}
