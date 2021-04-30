using Application;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class MovieTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public MovieTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_movies")
                .Options;
        }

        [Theory]
        [InlineData("Test description", 1, 104, "2010-10-04", "Test title")]
        public async Task Create_ValidInput_ReturnsCorrectData(string description, int languageID, int length, string releaseDate, string title)
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            };

            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                Description = description,
                Language = new AdminLanguageModel { ID = languageID },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);

            // Act
            var result = await appMovie.Create(expectedMovie);

            // Assert
            Assert.NotEqual(expectedMovie.ID, result.ID);
            Assert.Equal(expectedMovie.Description, result.Description);
            Assert.Equal(language.ID, result.Language.ID);
            Assert.Equal(language.Name, result.Language.Name);
            Assert.Equal(expectedMovie.Length, result.Length);
            Assert.Equal(expectedMovie.ReleaseDate, result.ReleaseDate);
            Assert.Equal(expectedMovie.Title, result.Title);
        }

        [Theory]
        [InlineData("", 1, 114, "2010-10-04", "Test title")]                    // description = empty
        [InlineData(null, 1, 114, "2010-10-04", "Test title")]                  // description = null
        [InlineData("Test description", 0, 114, "2010-10-04", "Test title")]    // languageID = 0
        [InlineData("Test description", 1, 0, "2010-10-04", "Test title")]      // length = 0
        [InlineData("Test description", 1, 114, "", "Test title")]              // releaseDate = empty
        [InlineData("Test description", 1, 114, "2010-10-04", "")]              // title = empty
        [InlineData("Test description", 1, 114, "2010-10-04", null)]            // title = null
        public async Task Create_InvalidInput_ReturnsNull(string description, int languageID, int length, string releaseDate, string title)
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            };

            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            // '4-10-2010 12:12:12' simulates as an empty DateTime
            DateTime expectedReleaseDate = releaseDate == "" ? DateTime.Parse("4-10-2010 12:12:12") : DateTime.Parse(releaseDate);

            var expectedMovie = new AdminMovieModel
            {
                Description = description,
                Language = new AdminLanguageModel { ID = languageID },
                Length = length,
                ReleaseDate = expectedReleaseDate,
                Title = title
            };

            var appMovie = new Movie(dbContext);

            // Act
            var result = await appMovie.Create(expectedMovie);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            };

            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            var expectedMovie = new Domain.Movie
            {
                ID = id,
                Description = "Test description",
                LanguageID = language.ID,
                Length = 104,
                ReleaseDate = DateTime.Parse("4-10-2010").ToString(),
                Title = "Test title"
            };

            dbContext.Movies.Add(expectedMovie);
            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);

            // Act
            var result = appMovie.Read(id);

            // Assert
            Assert.Equal(expectedMovie.ID, result.ID);
            Assert.Equal(expectedMovie.Description, result.Description);
            Assert.Equal(language.ID, result.Language.ID);
            Assert.Equal(language.Name, result.Language.Name);
            Assert.Equal(expectedMovie.Length, result.Length);
            Assert.Equal(expectedMovie.ReleaseDate, result.ReleaseDate.ToString());
            Assert.Equal(expectedMovie.Title, result.Title);
        }

        [Fact]
        public async Task ReadAll_ReturnsAllMovies()
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);

            dbContext.Movies.AddRange(
                Enumerable.Range(1, 5).Select(m => new Domain.Movie { 
                    ID = m, 
                    Description = $"Description {m}", 
                    Length = 114,
                    ReleaseDate = DateTime.Parse("4-10-2010").ToShortDateString(),
                    Title = $"Title {m}"
                })
            );

            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);

            // Act
            var result = appMovie.ReadAll();

            // Assert
            var movieModel = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(result);
            Assert.Equal(5, movieModel.Count());
        }
    }
}
