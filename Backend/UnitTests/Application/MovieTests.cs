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
        [InlineData("Test description", 104, "2010-10-04", "Test title")]
        public async Task Create_ValidInput_ReturnsCorrectData(string description, int length, string releaseDate, string title)
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);

            await dbContext.Database.EnsureDeletedAsync();

            var expectedMovie = new AdminMovieModel
            {
                Description = description,
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
            Assert.Equal(expectedMovie.Length, result.Length);
            Assert.Equal(expectedMovie.ReleaseDate, result.ReleaseDate);
            Assert.Equal(expectedMovie.Title, result.Title);
        }

        [Theory]
        [InlineData("Test description", 0, "2010-10-04", "Test title")]
        [InlineData("", 114, "2010-10-04", "Test title")]
        [InlineData(null, 114, "2010-10-04", "Test title")]
        [InlineData("Test description", 114, "2010-10-04", "")]
        [InlineData("Test description", 114, "2010-10-04", null)]
        public async Task Create_InvalidInput_ReturnsNull(string description, int length, string releaseDate, string title)
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);

            await dbContext.Database.EnsureDeletedAsync();

            DateTime expectedReleaseDate = DateTime.Parse(releaseDate);

            var expectedMovie = new AdminMovieModel
            {
                Description = description,
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

            var expectedMovie = new Domain.Movie
            {
                ID = id,
                Description = "Test description",
                Length = 104,
                ReleaseDate = DateTime.Parse("2010-10-04"),
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
            Assert.Equal(expectedMovie.Length, result.Length);
            Assert.Equal(expectedMovie.ReleaseDate, result.ReleaseDate);
            Assert.Equal(expectedMovie.Title, result.Title);
        }

        [Fact]
        public async Task ReadAll_ReturnsAllMovies()
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);

            dbContext.Movies.AddRange(
                Enumerable.Range(1, 5).Select(m => new Domain.Movie { ID = m, Description = $"Description {m}" })
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
