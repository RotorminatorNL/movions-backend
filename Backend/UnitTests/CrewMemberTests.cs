using Application;
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

        public CrewMemberTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_crewMember")
                .Options;
        }

        [Theory]
        [InlineData("Character Name", CrewRoles.Actor)]
        public async Task Create_ValidInput_ReturnsCorrectData(string characterName, CrewRoles crewRole)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Create(expectedCrewMember);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidInputData()
        {
            string characterName = "Name";
            CrewRoles crewRole = CrewRoles.Actor;

            // characterName = null
            yield return new object[] { null, crewRole };
            // characterName = empty
            yield return new object[] { "", crewRole };
            // crewRole = 100 (does not exists)
            yield return new object[] { characterName, 100 };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidInputData))]
        public async Task Create_InvalidInput_ReturnNull(string characterName, CrewRoles crewRole)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole
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

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = "Character Name",
                Role = CrewRoles.Actor
            };

            var appCrewMember = new CrewMember(dbContext);

            await appCrewMember.Create(expectedCrewMember);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
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

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = "Name",
                Role = CrewRoles.Actor
            };

            var appCrewMember = new CrewMember(dbContext);

            await appCrewMember.Create(expectedCrewMember);
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

            int expectedAmount = 5;

            dbContext.CrewMembers.AddRange(
                Enumerable.Range(1, expectedAmount).Select(c => new Domain.CrewMember
                {
                    CrewMemberID = c,
                    CharacterName = $"Character Name {c}",
                    Role = CrewRoles.Actor
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
        [InlineData(1, "Character Name", CrewRoles.Writer)]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string characterName, CrewRoles crewRole)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor
            };
            dbContext.CrewMembers.Add(crewMember);

            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Update(expectedCrewMember);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            #endregion
        }

        public static IEnumerable<object[]> UpdateInvalidInputData()
        {
            int id = 1;
            string characterName = "Name";
            CrewRoles crewRole = CrewRoles.Writer;

            // id = 0
            yield return new object[] { 0, characterName, crewRole };
            // id = 2 (does not exist)
            yield return new object[] { 2, characterName, crewRole };
            // characterName = null
            yield return new object[] { id, null, crewRole };
            // characterName = empty
            yield return new object[] { id, "", crewRole };
            // crewRole = 100 (does not exist)
            yield return new object[] { id, characterName, 100 };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidInputData))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string characterName, CrewRoles crewRole)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Character Name",
                Role = CrewRoles.Actor
            };
            dbContext.CrewMembers.Add(crewMember);

            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole
            };

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Update(expectedCrewMember);
            #endregion

            #region Assert
            Assert.Null(actualCrewMember);
            #endregion
        }

        [Theory]
        [InlineData(1, "Character Name", CrewRoles.Actor)]
        public async Task Update_InputIsNotDifferent_ReturnsEmptyAdminCrewMemberModel(int id, string characterName, CrewRoles crewRole)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var crewMember = new Domain.CrewMember
            {
                CharacterName = characterName,
                Role = crewRole
            };
            dbContext.CrewMembers.Add(crewMember);

            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole
            };

            var expectedCrewMember = new AdminCrewMemberModel();

            var appCrewMember = new CrewMember(dbContext);
            #endregion

            #region Act
            var actualCrewMember = await appCrewMember.Update(newCrewMember);
            #endregion

            #region Assert
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
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
