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
                new List<int> { 1, 2 }, 
                "Test description",
                new List<int> { 1, 2 },
                1, 
                104, 
                "2010-10-04", 
                "Test title" 
            };
        }

        [Theory]
        [MemberData(nameof(ValidInputData))]
        public async Task Create_ValidInput_ReturnsCorrectData(IEnumerable<int> companyIDs, string description, IEnumerable<int> genreIDs, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var componies = new List<Domain.Company>
            {
                new Domain.Company
                {
                    Name = "Lionsgate",
                    Type = 0
                },
                new Domain.Company
                {
                    Name = "Millennium Films",
                    Type = (Domain.Company.Types)1
                }
            };

            var language = new Domain.Language
            {
                Name = "English"
            };

            var genres = new List<Domain.Genre>
            {
                new Domain.Genre
                {
                    Name = "Adventure"
                },
                new Domain.Genre
                {
                    Name = "Action"
                }
            };

            dbContext.Companies.Add(componies[0]);
            dbContext.Companies.Add(componies[1]);
            dbContext.Languages.Add(language);
            dbContext.Genres.Add(genres[0]);
            dbContext.Genres.Add(genres[1]);

            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                Companies = companyIDs.Select(companyID => new AdminCompanyModel
                {
                    ID = companyID
                }),
                Description = description,
                Genres = genreIDs.Select(genreID => new AdminGenreModel
                {
                    ID = genreID
                }),
                Language = new AdminLanguageModel { ID = languageID },
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
            Assert.NotEqual(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            // Companies
            Assert.Equal(expectedMovie.Companies.Count(), actualMovie.Companies.Count());
            Assert.Equal(expectedMovie.Companies.ToList()[0].ID, actualMovie.Companies.ToList()[0].ID);
            Assert.Equal(componies[0].Name, actualMovie.Companies.ToList()[0].Name);
            Assert.Equal(componies[0].Type.ToString(), actualMovie.Companies.ToList()[0].Type.ToString());
            Assert.Equal(expectedMovie.Companies.ToList()[1].ID, actualMovie.Companies.ToList()[1].ID);
            Assert.Equal(componies[1].Name, actualMovie.Companies.ToList()[1].Name);
            Assert.Equal(componies[1].Type.ToString(), actualMovie.Companies.ToList()[1].Type.ToString());
            // Language
            Assert.Equal(language.ID, actualMovie.Language.ID);
            Assert.Equal(language.Name, actualMovie.Language.Name);
            // Genres
            Assert.Equal(expectedMovie.Genres.Count(), actualMovie.Genres.Count());
            Assert.Equal(expectedMovie.Genres.ToList()[0].ID, actualMovie.Genres.ToList()[0].ID);
            Assert.Equal(genres[0].Name, actualMovie.Genres.ToList()[0].Name);
            Assert.Equal(expectedMovie.Genres.ToList()[1].ID, actualMovie.Genres.ToList()[1].ID);
            Assert.Equal(genres[1].Name, actualMovie.Genres.ToList()[1].Name);
            #endregion
        }

        public static IEnumerable<object[]> InvalidInputData()
        {
            // companies = null
            yield return new object[] {
                null,
                "Test description",
                new List<int> { 1, 2 },
                1,
                104,
                "2010-10-04",
                "Test title"
            };
            // companies = empty
            yield return new object[] {
                new List<int>(),
                "Test description",
                new List<int> { 1, 2 },
                1,
                104,
                "2010-10-04",
                "Test title"
            };
            // description = null
            yield return new object[] {
                new List<int> { 1, 2 },
                null,
                new List<int> { 1, 2 },
                1,
                104,
                "2010-10-04",
                "Test title"
            };
            // description = empty
            yield return new object[] {
                new List<int> { 1, 2 },
                "",
                new List<int> { 1, 2 },
                1,
                104,
                "2010-10-04",
                "Test title"
            };
            // genres = null
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                null,
                1,
                104,
                "2010-10-04",
                "Test title"
            };
            // genres = empty
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int>(),
                1,
                104,
                "2010-10-04",
                "Test title"
            };
            // language = null
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                null,
                104,
                "2010-10-04",
                "Test title"
            };
            // language = 0
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                0,
                104,
                "2010-10-04",
                "Test title"
            };
            // length = null
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                1,
                null,
                "2010-10-04",
                "Test title"
            };
            // length = 0
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                1,
                0,
                "2010-10-04",
                "Test title"
            };
            // releaseDate = null
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                1,
                104,
                null,
                "Test title"
            };
            // releaseDate = empty
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                1,
                104,
                "",
                "Test title"
            };
            // title = null
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                1,
                104,
                "2010-10-04",
                null
            };
            // title = null
            yield return new object[] {
                new List<int> { 1, 2 },
                "Test description",
                new List<int> { 1, 2 },
                1,
                104,
                "2010-10-04",
                ""
            };
        }

        [Theory]
        [MemberData(nameof(InvalidInputData))]
        public async Task Create_InvalidInput_ReturnsNull(IEnumerable<int> companyIDs, string description, IEnumerable<int> genreIDs, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var componies = new List<Domain.Company>
            {
                new Domain.Company
                {
                    Name = "Lionsgate",
                    Type = 0
                },
                new Domain.Company
                {
                    Name = "Millennium Films",
                    Type = (Domain.Company.Types)1
                }
            };

            var language = new Domain.Language
            {
                Name = "English"
            };

            var genres = new List<Domain.Genre>
            {
                new Domain.Genre
                {
                    Name = "Adventure"
                },
                new Domain.Genre
                {
                    Name = "Action"
                }
            };

            dbContext.Companies.Add(componies[0]);
            dbContext.Companies.Add(componies[1]);
            dbContext.Languages.Add(language);
            dbContext.Genres.Add(genres[0]);
            dbContext.Genres.Add(genres[1]);

            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                Description = description,
                Language = new AdminLanguageModel { ID = languageID },
                Length = length,
                Title = title
            };

            if (companyIDs != null)
            {
                expectedMovie.Companies = companyIDs.Select(companyID => new AdminCompanyModel
                {
                    ID = companyID
                });
            }

            if (genreIDs != null)
            {
                expectedMovie.Genres = genreIDs.Select(genreID => new AdminGenreModel
                {
                    ID = genreID
                });
            }

            if (releaseDate != null && releaseDate != "")
            {
                expectedMovie.ReleaseDate = DateTime.Parse(releaseDate);
            }

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
            #endregion

            #region Act
            var result = appMovie.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedMovie.ID, result.ID);
            Assert.Equal(expectedMovie.Description, result.Description);
            Assert.Equal(language.ID, result.Language.ID);
            Assert.Equal(language.Name, result.Language.Name);
            Assert.Equal(expectedMovie.Length, result.Length);
            Assert.Equal(expectedMovie.ReleaseDate, result.ReleaseDate.ToString());
            Assert.Equal(expectedMovie.Title, result.Title);
            #endregion
        }

        [Fact]
        public async Task ReadAll_ReturnsAllMovies()
        {
            #region Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

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
            #endregion

            #region Act
            var result = appMovie.ReadAll();
            #endregion

            #region Assert
            var movieModel = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(result);
            Assert.Equal(5, movieModel.Count());
            #endregion
        }
    }
}
