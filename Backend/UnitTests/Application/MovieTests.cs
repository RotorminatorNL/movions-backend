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
        [InlineData("Test description", 1, 104, "04-10-2010", "Test title")]
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


            Console.WriteLine("------------------------------------------");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("--- Test Console 'log' ---");

            Console.WriteLine("string releaseDate: " + releaseDate);
            Console.WriteLine("expectedMovie ReleaseDate: " + expectedMovie.ReleaseDate);

            Console.WriteLine("------------------------------------------");
            Console.WriteLine("------------------------------------------");

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Create(expectedMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidInputData()
        {
            string description = "Test description";
            int languageID = 1;
            int length = 104;
            string releaseDate = "04-10-2010";
            string title = "Test title";

            // description = null
            yield return new object[] { null, languageID, length, releaseDate, title };
            // description = empty
            yield return new object[] { "", languageID, length, releaseDate, title };
            // language = null
            yield return new object[] { description, null, length, releaseDate, title };
            // language = empty
            yield return new object[] { description, 0, length, releaseDate, title };
            // length = null (i.e. 0)
            yield return new object[] { description, languageID, 0, releaseDate, title };
            // releaseDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] { description, languageID, length, "1-1-0001 00:00:00", title };
            // title = null
            yield return new object[] { description, languageID, length, releaseDate, null };
            // title = empty
            yield return new object[] { description, languageID, length, releaseDate, "" };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidInputData))]
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

            var expectedMovie = new AdminMovieModel
            {
                ID = id,
                Description = "Test description",
                Language = new AdminLanguageModel
                {
                    ID = language.ID,
                    Name = language.Name
                },
                Length = 104,
                ReleaseDate = DateTime.Parse("04-10-2010"),
                Title = "Test title"
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

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task Read_InvalidInput_ReturnsNull(int id)
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
                ID = id,
                Description = "Test description",
                Language = new AdminLanguageModel
                {
                    ID = language.ID,
                    Name = language.Name
                },
                Length = 104,
                ReleaseDate = DateTime.Parse("04-10-2010"),
                Title = "Test title"
            };

            var appMovie = new Movie(dbContext);

            await appMovie.Create(expectedMovie);
            #endregion

            #region Act
            var actualMovie = appMovie.Read(id);
            #endregion

            #region Assert
            Assert.Null(actualMovie);
            #endregion
        }

        [Fact]
        public async Task ReadAll_MoviesExist_ReturnsAllMovies()
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

        [Fact]
        public async Task ReadAll_NoMoviesExist_ReturnsEmptyList()
        {
            #region Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 0;

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

        [Theory]
        [InlineData(1, "Test description", 2, 104, "04-10-2010", "Test title")]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            }; 
            var language2 = new Domain.Language
            {
                Name = "Dutch"
            };
            dbContext.Languages.Add(language);
            dbContext.Languages.Add(language2);

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = 1,
                Length = 10,
                ReleaseDate = "10-10-2010",
                Title = "Title"
            };
            dbContext.Movies.Add(movie);

            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                Language = new AdminLanguageModel
                {
                    ID = languageID,
                    Name = language.Name
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Update(expectedMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            #endregion
        }

        public static IEnumerable<object[]> UpdateInvalidInputData()
        {
            int id = 1;
            string description = "Description";
            int languageID = 1;
            int length = 10;
            string releaseDate = "04-10-2010";
            string title = "Title";

            // id = 0
            yield return new object[] { 0, description, languageID, length, releaseDate, title };
            // id = 2 (does not exist)
            yield return new object[] { 2, description, languageID, length, releaseDate, title };
            // description = null
            yield return new object[] { id, null, languageID, length, releaseDate, title };
            // description = empty
            yield return new object[] { id, "", languageID, length, releaseDate, title };
            // language = null
            yield return new object[] { id, description, null, length, releaseDate, title };
            // language = empty
            yield return new object[] { id, description, 0, length, releaseDate, title };
            // length = null (i.e. 0)
            yield return new object[] { id, description, languageID, 0, releaseDate, title };
            // releaseDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] { id, description, languageID, length, "1-1-0001 00:00:00", title };
            // title = null
            yield return new object[] { id, description, languageID, length, releaseDate, null };
            // title = empty
            yield return new object[] { id, description, languageID, length, releaseDate, "" };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidInputData))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            };
            var language2 = new Domain.Language
            {
                Name = "Dutch"
            };
            dbContext.Languages.Add(language);
            dbContext.Languages.Add(language2);

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = 1,
                Length = 10,
                ReleaseDate = "10-10-2010",
                Title = "Title"
            };
            dbContext.Movies.Add(movie);

            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                Language = new AdminLanguageModel
                {
                    ID = languageID,
                    Name = language.Name
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Update(expectedMovie);
            #endregion

            #region Assert
            Assert.Null(actualMovie);
            #endregion
        }

        [Theory]
        [InlineData(1, "Description", 1, 10, "04-10-2010", "Title")]
        public async Task Update_InputIsNotDifferent_ReturnsEmptyAdminMovieModel(int id, string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "English"
            };
            var language2 = new Domain.Language
            {
                Name = "Dutch"
            };
            dbContext.Languages.Add(language);
            dbContext.Languages.Add(language2);

            var movie = new Domain.Movie
            {
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = releaseDate,
                Title = title
            };
            dbContext.Movies.Add(movie);

            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                Language = new AdminLanguageModel
                {
                    ID = languageID
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var expectedMovie = new AdminMovieModel();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Update(newMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actual = await appMovie.Delete(id);
            #endregion

            #region Assert
            Assert.True(actual);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task Delete_InvalidInput_ReturnsFalse(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actual = await appMovie.Delete(id);
            #endregion

            #region Assert
            Assert.False(actual);
            #endregion
        }
    }
}
