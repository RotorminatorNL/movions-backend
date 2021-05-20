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

        public static IEnumerable<object[]> ValidInputData()
        {
            yield return new object[] { 
                "Test description",
                1, 
                104, 
                "2010-10-04", 
                "Test title" 
            };
        }

        [Theory]
        [MemberData(nameof(ValidInputData))]
        public async Task Create_ValidInput_ReturnsCorrectData(string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
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
                ID = 1,
                Description = description,
                Language = new AdminLanguageModel { 
                    ID = languageID
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Create(expectedMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            Assert.NotNull(actualMovie.Language);
            #endregion
        }

        public static IEnumerable<object[]> InvalidInputData()
        {
            string description = "Test description";
            int languageID = 1;
            int length = 104;
            string releaseDate = "2010-10-04";
            string title = "Test title";

            // description = null
            yield return new object[] {
                null,
                languageID,
                length,
                releaseDate,
                title
            };
            // description = empty
            yield return new object[] {
                "",
                languageID,
                length,
                releaseDate,
                title
            };
            // language = null
            yield return new object[] {
                description,
                null,
                length,
                releaseDate,
                title
            };
            // language = empty
            yield return new object[] {
                description,
                0,
                length,
                releaseDate,
                title
            };
            // length = null (i.e. 0)
            yield return new object[] {
                description,
                languageID,
                0,
                releaseDate,
                title
            };
            // releaseDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] {
                description,
                languageID,
                length,
                "1-1-0001 00:00:00",
                title
            };
            // title = null
            yield return new object[] {
                description,
                languageID,
                length,
                releaseDate,
                null
            };
            // title = empty
            yield return new object[] {
                description,
                languageID,
                length,
                releaseDate,
                ""
            };
        }

        [Theory]
        [MemberData(nameof(InvalidInputData))]
        public async Task Create_InvalidInput_ReturnsNull(string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
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
                Language = languageID != 0 ? new AdminLanguageModel
                {
                    ID = languageID,
                    Name = language.Name
                } : null,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Create(expectedMovie);
            #endregion

            #region Assert
            Assert.Null(actualMovie);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            };
            dbContext.Languages.Add(language);

            await dbContext.SaveChangesAsync();

            string description = "Test description";
            int length = 104;
            string releaseDate = "2010-10-04";
            string title = "Test title";

            var expectedMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                Language = new AdminLanguageModel
                {
                    ID = language.ID,
                    Name = language.Name
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);

            await appMovie.Create(expectedMovie);
            #endregion

            #region Act
            var actualMovie = appMovie.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            #endregion
        }

        [Fact]
        public async Task ReadAll_ReturnsAllMovies()
        {
            #region Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 5;

            dbContext.Movies.AddRange(
                Enumerable.Range(1, expectedAmount).Select(m => new Domain.Movie { 
                    ID = m, 
                    Description = $"Description {m}", 
                    Length = 114,
                    ReleaseDate = DateTime.Parse("4-10-2010").ToShortDateString(),
                    Title = $"Title {m}"
                })
            );

            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var result = appMovie.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }
    }
}
