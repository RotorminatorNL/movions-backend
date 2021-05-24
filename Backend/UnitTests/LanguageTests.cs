using Application;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class LanguageTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public LanguageTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_language")
                .Options;
        }

        [Theory]
        [InlineData("Name")]
        public async Task Create_ValidInput_ReturnsCorrectData(string name)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var expectedLanguage = new AdminLanguageModel
            {
                ID = 1,
                Name = name
            };

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Create(expectedLanguage);
            #endregion

            #region Assert
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
            Assert.Equal(expectedLanguage.Name, actualLanguage.Name);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidInputData()
        {
            // name = null
            yield return new object[] { null };
            // name = empty
            yield return new object[] { "" };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidInputData))]
        public async Task Create_InvalidInput_ReturnNull(string name)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.SaveChangesAsync();

            var expectedLanguage = new AdminLanguageModel
            {
                ID = 1,
                Name = name
            };

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Create(expectedLanguage);
            #endregion

            #region Assert
            Assert.Null(actualLanguage);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var expectedLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = "Name"
            };

            var appLanguage = new Language(dbContext);

            await appLanguage.Create(expectedLanguage);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
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

            var expectedLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = "Name"
            };

            var appLanguage = new Language(dbContext);

            await appLanguage.Create(expectedLanguage);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Read(id);
            #endregion

            #region Assert
            Assert.Null(actualLanguage);
            #endregion
        }

        [Fact]
        public async Task ReadAll_LanguagesExist_ReturnsAllLanguages()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 5;

            dbContext.Languages.AddRange(
                Enumerable.Range(1, expectedAmount).Select(c => new Domain.Language
                {
                    ID = c,
                    Name = $"Name {c}"
                })
            );

            await dbContext.SaveChangesAsync();

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var result = await appLanguage.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<LanguageModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoLanguagesExist_ReturnsEmptyList()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 0;

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var result = await appLanguage.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<LanguageModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Theory]
        [InlineData(1, "Name")]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string name)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "Test"
            };
            dbContext.Languages.Add(language);

            await dbContext.SaveChangesAsync();

            var expectedLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = name
            };

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Update(expectedLanguage);
            #endregion

            #region Assert
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
            Assert.Equal(expectedLanguage.Name, actualLanguage.Name);
            #endregion
        }

        public static IEnumerable<object[]> UpdateInvalidInputData()
        {
            int id = 1;
            string name = "Name";

            // id = 0
            yield return new object[] { 0, name };
            // id = 2 (does not exist)
            yield return new object[] { 2, name };
            // name = null
            yield return new object[] { id, null };
            // name = empty
            yield return new object[] { id, "" };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidInputData))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string name)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = "Test"
            };
            dbContext.Languages.Add(language);

            await dbContext.SaveChangesAsync();

            var expectedLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = name
            };

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Update(expectedLanguage);
            #endregion

            #region Assert
            Assert.Null(actualLanguage);
            #endregion
        }

        [Theory]
        [InlineData(1, "Name")]
        public async Task Update_InputIsNotDifferent_ReturnsEmptyAdminLanguageModel(int id, string name)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language
            {
                Name = name
            };
            dbContext.Languages.Add(language);

            await dbContext.SaveChangesAsync();

            var newLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = name
            };

            var expectedLanguage = new AdminLanguageModel();

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actualLanguage = await appLanguage.Update(newLanguage);
            #endregion

            #region Assert
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var language = new Domain.Language();
            dbContext.Languages.Add(language);

            await dbContext.SaveChangesAsync();

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actual = await appLanguage.Delete(id);
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

            var language = new Domain.Language();
            dbContext.Languages.Add(language);

            await dbContext.SaveChangesAsync();

            var appLanguage = new Language(dbContext);
            #endregion

            #region Act
            var actual = await appLanguage.Delete(id);
            #endregion

            #region Assert
            Assert.False(actual);
            #endregion
        }
    }
}
