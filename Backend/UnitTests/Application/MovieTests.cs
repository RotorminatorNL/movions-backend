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
                new List<AdminCrewRoleModel> 
                {
                    new AdminCrewRoleModel
                    {
                        ID = 1,
                        CharacterName = "Barney Ross",
                        PersonID = 1,
                        Role = AdminCrewRoleModel.Roles.Actor
                    },
                    new AdminCrewRoleModel
                    {
                        ID = 2,
                        PersonID = 1,
                        Role = AdminCrewRoleModel.Roles.Director
                    },
                    new AdminCrewRoleModel
                    {
                        ID = 3,
                        PersonID = 1,
                        Role = AdminCrewRoleModel.Roles.Writer
                    }
                },
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
        public async Task Create_ValidInput_ReturnsCorrectData(IEnumerable<int> companyIDs, IEnumerable<AdminCrewRoleModel> crew, string description, IEnumerable<int> genreIDs, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company { 
                Name = "Lionsgate", 
                Type = 0 
            };

            var person = new Domain.Person
            {
                BirthDate = "1946-7-6",
                BirthPlace = "New York City, New York, USA",
                Description = "Michael Sylvester Gardenzio Stallone was born in the Hell's Kitchen neighborhood of Manhattan, New York City on July 6, 1946, the elder son of Francesco \"Frank\" Stallone Sr., a hairdresser and beautician, and Jacqueline \"Jackie\" Stallone (née Labofish; 1921–2020), an astrologer, dancer, and promoter of women's wrestling. His Italian father was born in Gioia del Colle, Italy and moved to the U.S. in the 1930s, while his American mother is of French (from Brittany) and Eastern European descent. His younger brother is actor and musician Frank Stallone.",
                FirstName = "Sylvester",
                LastName = "Stallone"
            };

            var language = new Domain.Language
            {
                Name = "English"
            };

            var genre = new Domain.Genre
            {
                Name = "Adventure"
            };

            dbContext.Companies.Add(company);
            dbContext.Persons.Add(person);
            dbContext.Languages.Add(language);
            dbContext.Genres.Add(genre);

            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                Companies = companyIDs.Select(companyID => new AdminCompanyModel
                {
                    ID = companyID
                }),
                Crew = crew.Select(crewMember => new AdminCrewRoleModel
                {
                    ID = crewMember.ID,
                    CharacterName = crewMember.CharacterName,
                    PersonID = crewMember.PersonID,
                    Role = crewMember.Role
                }),
                Description = description,
                Genres = genreIDs.Select(genreID => new AdminGenreModel
                {
                    ID = genreID
                }),
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
            Assert.NotEqual(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);

            Assert.NotEmpty(actualMovie.Companies);
            
            Assert.NotEmpty(actualMovie.Crew);

            Assert.NotNull(actualMovie.Language);
            
            Assert.NotEmpty(actualMovie.Genres);
            #endregion
        }

        public static IEnumerable<object[]> InvalidInputData()
        {
            var companyIDs = new List<int>
            {
                1,
                2
            };
            var crew = new List<AdminCrewRoleModel>
            {
                new AdminCrewRoleModel
                {
                    ID = 1,
                    CharacterName = "Barney Ross",
                    PersonID = 1,
                    Role = AdminCrewRoleModel.Roles.Actor
                },
                new AdminCrewRoleModel
                {
                    ID = 2,
                    PersonID = 1,
                    Role = AdminCrewRoleModel.Roles.Director
                },
                new AdminCrewRoleModel
                {
                    ID = 3,
                    PersonID = 1,
                    Role = AdminCrewRoleModel.Roles.Writer
                }
            };
            string description = "Test description";
            var genreIDs = new List<int>
            {
                1,
                2
            };
            int languageID = 1;
            int length = 104;
            string releaseDate = "2010-10-04";
            string title = "Test title";

            // companies = null
            yield return new object[] {
                null,
                crew,
                description,
                genreIDs,
                languageID,
                length,
                releaseDate,
                title
            };
            // companies = empty
            yield return new object[] {
                new List<int>(),
                crew,
                description,
                genreIDs,
                languageID,
                length,
                releaseDate,
                title
            };
            // crew = null
            yield return new object[] {
                companyIDs,
                null,
                description,
                genreIDs,
                languageID,
                length,
                releaseDate,
                title
            };
            // crew = empty
            yield return new object[] {
                companyIDs,
                new List<AdminCrewRoleModel>(),
                description,
                genreIDs,
                languageID,
                length,
                releaseDate,
                title
            };
            // description = null
            yield return new object[] {
                companyIDs,
                crew,
                null,
                genreIDs,
                languageID,
                length,
                releaseDate,
                title
            };
            // description = empty
            yield return new object[] {
                companyIDs,
                crew,
                "",
                genreIDs,
                languageID,
                length,
                releaseDate,
                title
            };
            // genres = null
            yield return new object[] {
                companyIDs,
                crew,
                description,
                null,
                languageID,
                length,
                releaseDate,
                title
            };
            // genres = empty
            yield return new object[] {
                companyIDs,
                crew,
                description,
                new List<int>(),
                languageID,
                length,
                releaseDate,
                title
            };
            // language = null
            yield return new object[] {
                companyIDs,
                crew,
                description,
                genreIDs,
                null,
                length,
                releaseDate,
                title
            };
            // language = empty
            yield return new object[] {
                companyIDs,
                crew,
                description,
                genreIDs,
                0,
                length,
                releaseDate,
                title
            };
            // length = null (i.e. 0)
            yield return new object[] {
                companyIDs,
                crew,
                description,
                genreIDs,
                languageID,
                0,
                releaseDate,
                title
            };
            // releaseDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] {
                companyIDs,
                crew,
                description,
                genreIDs,
                languageID,
                length,
                "1-1-0001 00:00:00",
                title
            };
            // title = null
            yield return new object[] {
                companyIDs,
                crew,
                description,
                genreIDs,
                languageID,
                length,
                releaseDate,
                null
            };
            // title = empty
            yield return new object[] {
                companyIDs,
                crew,
                description,
                genreIDs,
                languageID,
                length,
                releaseDate,
                ""
            };
        }

        [Theory]
        [MemberData(nameof(InvalidInputData))]
        public async Task Create_InvalidInput_ReturnsNull(IEnumerable<int> companyIDs, IEnumerable<AdminCrewRoleModel> crew, string description, IEnumerable<int> genreIDs, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var companies = new List<Domain.Company>
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

            var persons = new List<Domain.Person>
            {
                new Domain.Person
                {
                    BirthDate = "1946-7-6",
                    BirthPlace = "New York City, New York, USA",
                    Description = "Michael Sylvester Gardenzio Stallone was born in the Hell's Kitchen neighborhood of Manhattan, New York City on July 6, 1946, the elder son of Francesco \"Frank\" Stallone Sr., a hairdresser and beautician, and Jacqueline \"Jackie\" Stallone (née Labofish; 1921–2020), an astrologer, dancer, and promoter of women's wrestling. His Italian father was born in Gioia del Colle, Italy and moved to the U.S. in the 1930s, while his American mother is of French (from Brittany) and Eastern European descent. His younger brother is actor and musician Frank Stallone.",
                    FirstName = "Sylvester",
                    LastName = "Stallone"
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

            dbContext.Companies.Add(companies[0]);
            dbContext.Companies.Add(companies[1]);
            dbContext.Persons.Add(persons[0]);
            dbContext.Languages.Add(language);
            dbContext.Genres.Add(genres[0]);
            dbContext.Genres.Add(genres[1]);

            await dbContext.SaveChangesAsync();

            var expectedMovie = new AdminMovieModel
            {
                Companies = companyIDs?.Select(companyID => new AdminCompanyModel
                {
                    ID = companyID,
                    Name = companies[companyID - 1].Name,
                    Type = (AdminCompanyModel.Types)companies[companyID - 1].Type
                }),
                Crew = crew?.Select(crewMember => new AdminCrewRoleModel
                {
                    ID = crewMember.ID,
                    CharacterName = crewMember.CharacterName,
                    PersonID = crewMember.PersonID,
                    Person = new AdminPersonModel
                    {
                        ID = persons[0].ID,
                        FirstName = persons[0].FirstName,
                        LastName = persons[0].LastName
                    },
                    Role = crewMember.Role
                }),
                Description = description,
                Genres = genreIDs?.Select(genreID => new AdminGenreModel
                {
                    ID = genreID,
                    Name = genres[genreID - 1].Name
                }),
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

            var companies = new List<Domain.Company>
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

            var persons = new List<Domain.Person>
            {
                new Domain.Person
                {
                    BirthDate = "1946-7-6",
                    BirthPlace = "New York City, New York, USA",
                    Description = "Michael Sylvester Gardenzio Stallone was born in the Hell's Kitchen neighborhood of Manhattan, New York City on July 6, 1946, the elder son of Francesco \"Frank\" Stallone Sr., a hairdresser and beautician, and Jacqueline \"Jackie\" Stallone (née Labofish; 1921–2020), an astrologer, dancer, and promoter of women's wrestling. His Italian father was born in Gioia del Colle, Italy and moved to the U.S. in the 1930s, while his American mother is of French (from Brittany) and Eastern European descent. His younger brother is actor and musician Frank Stallone.",
                    FirstName = "Sylvester",
                    LastName = "Stallone"
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

            dbContext.Companies.Add(companies[0]);
            dbContext.Companies.Add(companies[1]);
            dbContext.Persons.Add(persons[0]);
            dbContext.Languages.Add(language);
            dbContext.Genres.Add(genres[0]);
            dbContext.Genres.Add(genres[1]);

            await dbContext.SaveChangesAsync();

            var crew = new List<AdminCrewRoleModel>
            {
                new AdminCrewRoleModel
                {
                    ID = 1,
                    CharacterName = "Barney Ross",
                    PersonID = 1,
                    Role = AdminCrewRoleModel.Roles.Actor
                },
                new AdminCrewRoleModel
                {
                    ID = 2,
                    PersonID = 1,
                    Role = AdminCrewRoleModel.Roles.Director
                },
                new AdminCrewRoleModel
                {
                    ID = 3,
                    PersonID = 1,
                    Role = AdminCrewRoleModel.Roles.Writer
                }
            };
            string description = "Test description";
            int length = 104;
            string releaseDate = "2010-10-04";
            string title = "Test title";

            var expectedMovie = new AdminMovieModel
            {
                ID = id,
                Companies = companies.Select(company => new AdminCompanyModel
                {
                    ID = company.ID,
                    Name = company.Name,
                    Type = (AdminCompanyModel.Types)company.Type
                }),
                Crew = crew.Select(crewMember => new AdminCrewRoleModel
                {
                    ID = crewMember.ID,
                    CharacterName = crewMember.CharacterName,
                    PersonID = crewMember.PersonID,
                    Person = new AdminPersonModel
                    {
                        ID = persons[0].ID,
                        FirstName = persons[0].FirstName,
                        LastName = persons[0].LastName
                    },
                    Role = crewMember.Role
                }),
                Description = description,
                Genres = genres.Select(genre => new AdminGenreModel
                {
                    ID = genre.ID,
                    Name = genre.Name
                }),
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
            Assert.Equal(expectedMovie.Companies.ToList()[0].ID, actualMovie.Companies.ToList()[0].ID);
            Assert.Equal(expectedMovie.Companies.ToList()[1].ID, actualMovie.Companies.ToList()[1].ID);
            Assert.Equal(expectedMovie.Crew.ToList()[0].ID, actualMovie.Crew.ToList()[0].ID);
            Assert.Equal(expectedMovie.Crew.ToList()[1].ID, actualMovie.Crew.ToList()[1].ID);
            Assert.Equal(expectedMovie.Crew.ToList()[2].ID, actualMovie.Crew.ToList()[2].ID);
            Assert.Equal(expectedMovie.Crew.ToList()[0].Person.ID, actualMovie.Crew.ToList()[0].Person.ID);
            Assert.Equal(expectedMovie.Genres.ToList()[0].ID, actualMovie.Genres.ToList()[0].ID);
            Assert.Equal(expectedMovie.Genres.ToList()[1].ID, actualMovie.Genres.ToList()[1].ID);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
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
