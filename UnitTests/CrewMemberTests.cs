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
                Name = "Title"
            };

            var person = new Domain.Person
            {
                FirstName = "First Name",
                LastName = "Last Name"
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
                    Name = movie.Name
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
            Assert.Equal(expectedCrewMember.Movie.Name, actualCrewMember.Movie.Name);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnNull()
        {
            string characterName = "Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;
            int movieID = 1;
            int personID = 1;

            // Actor's character name cannot be null
            yield return new object[] { null, crewRoleActor, movieID, personID };
            // Actor's character name cannot be empty
            yield return new object[] { "", crewRoleActor, movieID, personID };
            // Director's character name must be null
            yield return new object[] { characterName, crewRoleDirector, movieID, personID };
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

            var newCrewMember = new AdminCrewMemberModel
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
            var actualCrewMember = await appCrewMember.Create(newCrewMember);
            #endregion

            #region Assert
            Assert.Null(actualCrewMember);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnsCrewMemberModelWithErrorID()
        {
            string characterName = "Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 2 (does not exist)
            yield return new object[] { characterName, crewRoleActor, 2, personID, -1 };
            // PersonID = 2 (does not exist)
            yield return new object[] { characterName, crewRoleActor, movieID, 2, -2 };
            // MovieID = 2 (does not exist)
            // PersonID = 2 (does not exist)
            yield return new object[] { characterName, crewRoleActor, 2, 2, -3 };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidInput_ReturnsCrewMemberModelWithErrorID))]
        public async Task Create_InvalidInput_ReturnsCrewMemberModelWithErrorID(string characterName, CrewRoles crewRole, int movieID, int personID, int expectedCrewMemberID)
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
            Assert.Equal(actualCrewMember.ID, expectedCrewMemberID);
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
                CharacterName = "Character Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            };

            dbContext.CrewMembers.Add(crewMember);
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new CrewMemberModel
            {
                ID = id,
                CharacterName = crewMember.CharacterName,
                Role = crewMember.Role.ToString(),
                Movie = new MovieModel
                {
                    ID = movie.ID,
                    Name = movie.Name
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
            Assert.Equal(expectedCrewMember.Movie.Name, actualCrewMember.Movie.Name);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_InvalidInput_ReturnsNull(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

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

            int expectedAmount = 2;

            dbContext.CrewMembers.AddRange(
                Enumerable.Range(1, expectedAmount).Select(x => new Domain.CrewMember 
                { 
                    CharacterName = $"Character Name {x}",
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
        [InlineData(1, null, CrewRoles.Writer, 2, 2)]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var data = await CreateMovieAndPerson(dbContext);
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var data2 = await CreateMovieAndPerson(dbContext);
            var movie2 = data2[0] as Domain.Movie;
            var person2 = data2[1] as Domain.Person;

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

            var expectedCrewMember = new CrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole.ToString(),
                Movie = new MovieModel
                {
                    ID = movie2.ID,
                    Name = movie2.Name
                },
                Person = new PersonModel
                {
                    ID = person2.ID,
                    FirstName = person2.FirstName,
                    LastName = person2.LastName
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
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Movie.Name, actualCrewMember.Movie.Name);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidInput_ReturnsNull()
        {
            int id = 1;
            string characterName = "New Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleWriter = CrewRoles.Writer;
            int movieID = 1;
            int personID = 1;

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
            string characterName = "New Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 2 (does not exist)
            yield return new object[] { id, characterName, crewRoleActor, 2, personID, -1 };
            // PersonID = 2 (does not exist)
            yield return new object[] { id, characterName, crewRoleActor, movieID, 2, -2 };
            // MovieID = 2 (does not exist)
            // PersonID = 2 (does not exist)
            yield return new object[] { id, characterName, crewRoleActor, 2, 2, -3 };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidInput_ReturnsCrewMemberModelWithErrorID))]
        public async Task Update_InvalidInput_ReturnsCrewMemberModelWithErrorID(int id, string characterName, CrewRoles crewRole, int movieID, int personID, int expectedCrewMemberID)
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
            Assert.Equal(actualCrewMember.ID, expectedCrewMemberID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            dbContext.CrewMembers.Add(new Domain.CrewMember());
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
        [InlineData(1)]
        public async Task Delete_InvalidInput_ReturnsFalse(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

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
