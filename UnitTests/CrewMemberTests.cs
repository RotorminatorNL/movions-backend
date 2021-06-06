using Application;
using Application.AdminModels;
using Application.ViewModels;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class CrewMemberTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        private async Task<object[]> CreateMovieAndPerson(ApplicationDbContext dbContext)
        {
            var movie = new Domain.Movie
            {
                Title = "Epic Movie"
            };

            var person = new Domain.Person
            {
                FirstName = "Don",
                LastName = "Diablo"
            };

            dbContext.Movies.Add(movie);
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            return new object[] { movie, person };
        }

        public CrewMemberTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_crewMember")
                .Options;
        }

        [Theory]
        [InlineData("Character Name", CrewRoles.Actor, 1, 1)]
        [InlineData(null, CrewRoles.Director, 1, 1)]
        public async Task Create_ValidInput_ReturnsCorrectData(string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var newCrewMember = new AdminCrewMemberModel
            {
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var expectedCrewMember = new CrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole.ToString(),
                Movie = new MovieModel
                {
                    ID = movie.ID,
                    Title = movie.Title
                },
                Person = new PersonModel
                {
                    ID = person.ID,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                }
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Create(newCrewMember);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Movie.Title, actualCrewMember.Movie.Title);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnNull()
        {
            string characterName = "Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;
            int movieID = 1;
            int personID = 1;

            // Actor's character name cannot be null
            yield return new object[] { null, crewRoleActor, movieID, personID };
            // Actor's character name cannot be empty
            yield return new object[] { "", crewRoleActor, movieID, personID };
            // Director's character name must be null
            yield return new object[] { "Epic Name", crewRoleDirector, movieID, personID };
            // Crew role must exist
            yield return new object[] { characterName, 100, movieID, personID };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidInput_ReturnNull))]
        public async Task Create_InvalidInput_ReturnNull(string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await CreateMovieAndPerson(dbContext);

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Create(expectedCrewMember);
            #endregion

            #region Assert
            Assert.Null(actualCrewMember);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnsCrewMemberModelWithErrorID()
        {
            string characterName = "Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 0
            yield return new object[] 
            { 
                characterName, crewRoleActor, 0, personID,
                new CrewMemberModel 
                {
                    ID = -1
                }
            };
            // PersonID = 0
            yield return new object[] 
            { 
                characterName, crewRoleActor, movieID, 0,
                new CrewMemberModel
                {
                    ID = -2
                }
            };
            // MovieID = 0 | PersonID = 0
            yield return new object[] 
            { 
                characterName, crewRoleActor, 0, 0,
                new CrewMemberModel
                {
                    ID = -3
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidInput_ReturnsCrewMemberModelWithErrorID))]
        public async Task Create_InvalidInput_ReturnsCrewMemberModelWithErrorID(string characterName, CrewRoles crewRole, int movieID, int personID, CrewMemberModel expectedCrewMember)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await CreateMovieAndPerson(dbContext);

            var newCrewMember = new AdminCrewMemberModel
            {
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Create(newCrewMember);
            #endregion

            #region Assert
            Assert.Equal(actualCrewMember.ID, expectedCrewMember.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Diego Lopez",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            };

            dbContext.CrewMembers.Add(crewMember);
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new CrewMemberModel
            {
                ID = id,
                CharacterName = "Diego Lopez",
                Role = CrewRoles.Actor.ToString(),
                Movie = new MovieModel
                {
                    ID = movie.ID,
                    Title = movie.Title
                },
                Person = new PersonModel
                {
                    ID = person.ID,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                }
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Movie.Title, actualCrewMember.Movie.Title);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
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

            await CreateMovieAndPerson(dbContext);

            dbContext.CrewMembers.Add(new Domain.CrewMember());
            await dbContext.SaveChangesAsync();

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Read(id);
            #endregion

            #region Assert
            Assert.Null(actualCrewMember);
            #endregion
        }

        [Fact]
        public async Task ReadAll_CrewMembersExist_ReturnsAllCrewMembers()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            int expectedAmount = 5;

            dbContext.CrewMembers.AddRange(
                Enumerable.Range(1, expectedAmount).Select(c => new Domain.CrewMember 
                { 
                    CharacterName = "Diego Lopez",
                    Role = CrewRoles.Actor,
                    MovieID = movie.ID,
                    PersonID = person.ID
                })
            );

            await dbContext.SaveChangesAsync();

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var result = await appCrewMember.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<CrewMemberModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoCrewMembersExist_ReturnsEmptyList()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 0;

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var result = await appCrewMember.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<CrewMemberModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Theory]
        [InlineData(1, null, CrewRoles.Writer, 1, 1)]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            };

            dbContext.CrewMembers.Add(crewMember);
            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var expectedCrewMember = new CrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole.ToString(),
                Movie = new MovieModel
                {
                    ID = movie.ID,
                    Title = movie.Title
                },
                Person = new PersonModel
                {
                    ID = person.ID,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                }
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Update(newCrewMember);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidInput_ReturnsNull()
        {
            int id = 1;
            string characterName = "Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleWriter = CrewRoles.Writer;
            int movieID = 1;
            int personID = 1;

            // id = 0
            yield return new object[] { 0, characterName, crewRoleWriter, movieID, personID };
            // id = 2 (does not exist)
            yield return new object[] { 2, characterName, crewRoleWriter, movieID, personID };
            // Writer's character name must be null
            yield return new object[] { id, characterName, crewRoleWriter, movieID, personID };
            // Actor's character name cannot be null
            yield return new object[] { id, null, crewRoleActor, movieID, personID };
            // Actor's character name cannot be empty
            yield return new object[] { id, "", crewRoleActor, movieID, personID };
            // Crew role must exist
            yield return new object[] { id, characterName, 100, movieID, personID };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidInput_ReturnsNull))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Character Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            };

            dbContext.CrewMembers.Add(crewMember);
            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Update(newCrewMember);
            #endregion

            #region Assert
            Assert.Null(actualCrewMember);
            #endregion
        }


        public static IEnumerable<object[]> Data_Update_InvalidInput_ReturnsCrewMemberModelWithErrorID()
        {
            int id = 1;
            string characterName = "Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 0
            yield return new object[]
            {
                id, characterName, crewRoleActor, 0, personID,
                new CrewMemberModel
                {
                    ID = -1
                }
            };
            // PersonID = 0
            yield return new object[]
            {
                id, characterName, crewRoleActor, movieID, 0,
                new CrewMemberModel
                {
                    ID = -2
                }
            };
            // MovieID = 0 | PersonID = 0
            yield return new object[]
            {
                id, characterName, crewRoleActor, 0, 0,
                new CrewMemberModel
                {
                    ID = -3
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidInput_ReturnsCrewMemberModelWithErrorID))]
        public async Task Update_InvalidInput_ReturnsCrewMemberModelWithErrorID(int id, string characterName, CrewRoles crewRole, int movieID, int personID, CrewMemberModel expectedCrewMember)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Character Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            };

            dbContext.CrewMembers.Add(crewMember);
            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Update(newCrewMember);
            #endregion

            #region Assert
            Assert.Equal(actualCrewMember.ID, expectedCrewMember.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var crewMember = new Domain.CrewMember();
            dbContext.CrewMembers.Add(crewMember);

            await dbContext.SaveChangesAsync();

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actual = await appCrewMember.Delete(id);
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

            var crewMember = new Domain.CrewMember();
            dbContext.CrewMembers.Add(crewMember);

            await dbContext.SaveChangesAsync();

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actual = await appCrewMember.Delete(id);
            #endregion

            #region Assert
            Assert.False(actual);
            #endregion
        }
    }
}
