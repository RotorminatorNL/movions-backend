using Application;
using Application.AdminModels;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class MovieTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public MovieTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");

            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_movies")
                .Options;
        }

        [Theory]
        [InlineData("Description", 1, 104, "04-10-2010", "Title")]
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

            var newMovie = new AdminMovieModel
            {
                ID = 1,
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var expectedMovie = new MovieModel
            {
                ID = 1,
                Description = "Description",
                Language = new LanguageModel { 
                    ID = 1
                },
                Length = 104,
                ReleaseDate = DateTime.Parse("04-10-2010"),
                Title = "Title"
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Create(newMovie);
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

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnsNull()
        {
            string description = "Description";
            int languageID = 1;
            int length = 104;
            string releaseDate = "04-10-2010";
            string title = "Title";

            // Description = null
            yield return new object[] { null, languageID, length, releaseDate, title };
            // Description = empty
            yield return new object[] { "", languageID, length, releaseDate, title };
            // LanguageID = 0
            yield return new object[] { description, 0, length, releaseDate, title };
            // Length = null (i.e. 0)
            yield return new object[] { description, languageID, 0, releaseDate, title };
            // ReleaseDate = 1-1-0001 00:00:00
            yield return new object[] { description, languageID, length, "1-1-0001 00:00:00", title };
            // Title = null
            yield return new object[] { description, languageID, length, releaseDate, null };
            // Title = empty
            yield return new object[] { description, languageID, length, releaseDate, "" };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidInput_ReturnsNull))]
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

            var newMovie = new AdminMovieModel
            {
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Create(newMovie);
            #endregion

            #region Assert
            Assert.Null(actualMovie);
            #endregion
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task ConnectGenre_ValidInput_ReturnsCorrectData(int id, int genreID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "Name"
            };
            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            var genre = new Domain.Genre
            {
                Name = "Name"
            };

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = 1,
                Length = 104,
                ReleaseDate = "04-10-2010", 
                Title = "Title"
            };

            dbContext.Genres.Add(genre);
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var newGenreMovie = new AdminGenreMovieModel
            {
                GenreID = genreID,
                MovieID = id
            };

            var expectedMovie = new MovieModel
            {
                ID = 1,
                Description = "Description", 
                Genres = new List<GenreModel> 
                {
                    new GenreModel
                    {
                        ID = 1,
                        Name = "Name"
                    }
                },
                Language = new LanguageModel
                {
                    ID = 1,
                    Name = "Name"
                },
                Title = "Title",
                Length = 104,
                ReleaseDate = DateTime.Parse("04-10-2010")
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.ConnectGenre(newGenreMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            Assert.Equal(expectedMovie.Genres.Count(), actualMovie.Genres.Count());
            Assert.Equal(expectedMovie.Genres.ToList()[0].ID, actualMovie.Genres.ToList()[0].ID);
            Assert.Equal(expectedMovie.Genres.ToList()[0].Name, actualMovie.Genres.ToList()[0].Name);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, -3)]
        [InlineData(0, 1, -2)]
        [InlineData(1, 0, -1)]
        [InlineData(1, 1, -4)]
        [InlineData(1, 2, -1)]
        [InlineData(2, 1, -2)]
        [InlineData(2, 2, -3)]
        public async Task ConnectGenre_InvalidInput_ReturnsNull(int id, int genreID, int expectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var genre = new Domain.Genre();
            dbContext.Genres.Add(genre);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            if (expectedID == -4)
            {
                dbContext.GenreMovies.Add(new Domain.GenreMovie
                {
                    GenreID = genre.ID,
                    MovieID = movie.ID
                });
            }

            await dbContext.SaveChangesAsync();

            var newGenreMovie = new AdminGenreMovieModel
            {
                GenreID = genreID,
                MovieID = id
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.ConnectGenre(newGenreMovie);
            #endregion

            #region Assert
            Assert.NotNull(actualMovie);
            Assert.Equal(expectedID, actualMovie.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var genre = new Domain.Genre
            {
                Name = "Name"
            };
            dbContext.Genres.Add(genre);
            var language = new Domain.Language
            {
                Name = "Name"
            };
            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            dbContext.Movies.Add(new Domain.Movie
            {
                Description = "Description",
                Genres = new List<Domain.GenreMovie>
                {
                    new Domain.GenreMovie
                    {
                        GenreID = genre.ID
                    }
                },
                LanguageID = language.ID,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Title = "Title"
            });
            await dbContext.SaveChangesAsync();

            var expectedMovie = new MovieModel
            {
                ID = id,
                Description = "Description",
                Genres = new List<GenreModel> 
                {
                    new GenreModel
                    {
                        ID = 1
                    }
                },
                Language = new LanguageModel
                {
                    ID = 1
                },
                Length = 104,
                ReleaseDate = DateTime.Parse("04-10-2010"),
                Title = "Title"
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Genres.ToList()[0].ID, actualMovie.Genres.ToList()[0].ID);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
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

            dbContext.Movies.Add(new Domain.Movie
            {
                Description = "Test description",
                LanguageID = language.ID,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Title = "Test title"
            });
            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Read(id);
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
            var result = await appMovie.ReadAll();
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
            var result = await appMovie.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Theory]
        [InlineData(1, "New Description", 2, 110, "10-10-2010", "New Title")]
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
            await dbContext.SaveChangesAsync();

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = 1,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Title = "Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var expectedMovie = new MovieModel
            {
                ID = 1,
                Description = "New Description",
                Language = new LanguageModel
                {
                    ID = 2
                },
                Length = 110,
                ReleaseDate = DateTime.Parse("10-10-2010"),
                Title = "New Title"
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Update(newMovie);
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

        public static IEnumerable<object[]> Data_Update_InvalidInput_ReturnsNull()
        {
            int id = 1;
            string description = "Description";
            int languageID = 1;
            int length = 10;
            string releaseDate = "04-10-2010";
            string title = "Title";

            // ID = 0
            yield return new object[] { 0, description, languageID, length, releaseDate, title };
            // ID = 2 (does not exist)
            yield return new object[] { 2, description, languageID, length, releaseDate, title };
            // Description = null
            yield return new object[] { id, null, languageID, length, releaseDate, title };
            // Description = empty
            yield return new object[] { id, "", languageID, length, releaseDate, title };
            // LanguageID = 0
            yield return new object[] { id, description, 0, length, releaseDate, title };
            // Length = 0
            yield return new object[] { id, description, languageID, 0, releaseDate, title };
            // ReleaseDate = 1-1-0001 00:00:00
            yield return new object[] { id, description, languageID, length, "1-1-0001 00:00:00", title };
            // Title = null
            yield return new object[] { id, description, languageID, length, releaseDate, null };
            // Title = empty
            yield return new object[] { id, description, languageID, length, releaseDate, "" };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidInput_ReturnsNull))]
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

            var newMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actualMovie = await appMovie.Update(newMovie);
            #endregion

            #region Assert
            Assert.Null(actualMovie);
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

        [Theory]
        [InlineData(1, 1, 0)]
        public async Task DisconnectGenre_ValidInput_ReturnsTrue(int id, int genreID, int expectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var genre = new Domain.Genre();
            dbContext.Genres.Add(genre);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            var genreMovie = new Domain.GenreMovie
            {
                GenreID = genre.ID,
                MovieID = movie.ID
            };
            dbContext.GenreMovies.Add(genreMovie);

            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actual = await appMovie.DisconnectGenre(new AdminGenreMovieModel { GenreID = genreID, MovieID = id });
            #endregion

            #region Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedID, actual.ID);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, -3)]
        [InlineData(0, 1, -2)]
        [InlineData(1, 0, -1)]
        [InlineData(1, 1, -4)]
        [InlineData(1, 2, -1)]
        [InlineData(2, 1, -2)]
        [InlineData(2, 2, -3)]
        public async Task DisconnectGenre_InvalidInput_ReturnsCompanyModelWithErrorID(int id, int genreID, int exptectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var genre = new Domain.Genre();
            dbContext.Genres.Add(genre);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            if (exptectedID != -4)
            {
                var genreMovie = new Domain.GenreMovie
                {
                    GenreID = genre.ID,
                    MovieID = movie.ID
                };
                dbContext.GenreMovies.Add(genreMovie);
            }
            await dbContext.SaveChangesAsync();

            var appMovie = new Movie(dbContext);
            #endregion

            #region Act
            var actual = await appMovie.DisconnectGenre(new AdminGenreMovieModel { GenreID = genreID, MovieID = id });
            #endregion

            #region Assert
            Assert.NotNull(actual);
            Assert.Equal(exptectedID, actual.ID);
            #endregion
        }
    }
}
