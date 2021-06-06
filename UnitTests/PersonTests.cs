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
    public class PersonTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public PersonTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");

            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_person")
                .Options;
        }

        [Theory]
        [InlineData("04-10-2010", "Birth Place", "Description", "First Name", "Last Name")]
        public async Task Create_ValidInput_ReturnsCorrectData(string birthDate, string birthPlace, string description, string firstName, string lastName)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var newPerson = new AdminPersonModel
            {
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var expectedPerson = new PersonModel
            {
                ID = 1,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actualPerson = await appPerson.Create(newPerson);
            #endregion

            #region Assert
            Assert.Equal(expectedPerson.ID, actualPerson.ID);
            Assert.Equal(expectedPerson.BirthDate, actualPerson.BirthDate);
            Assert.Equal(expectedPerson.BirthPlace, actualPerson.BirthPlace);
            Assert.Equal(expectedPerson.Description, actualPerson.Description);
            Assert.Equal(expectedPerson.FirstName, actualPerson.FirstName);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnNull()
        {
            var birthDate = "04-10-2010";
            var birthPlace = "Birth Place";
            var descripton = "Description";
            var firstName = "First Name";
            var lastName = "Last Name";

            // birthDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] { "1-1-0001 00:00:00", birthPlace, descripton, firstName, lastName };
            // birthPlace = null
            yield return new object[] { birthDate, null, descripton, firstName, lastName };
            // birthPlace = empty
            yield return new object[] { birthDate, "", descripton, firstName, lastName };
            // descripton = null
            yield return new object[] { birthDate, birthPlace, null, firstName, lastName };
            // descripton = empty
            yield return new object[] { birthDate, birthPlace, "", firstName, lastName };
            // firstName = null
            yield return new object[] { birthDate, birthPlace, descripton, null, lastName };
            // firstName = empty
            yield return new object[] { birthDate, birthPlace, descripton, "", lastName };
            // lastName = null
            yield return new object[] { birthDate, birthPlace, descripton, firstName, null };
            // lastName = empty
            yield return new object[] { birthDate, birthPlace, descripton, firstName, "" };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidInput_ReturnNull))]
        public async Task Create_InvalidInput_ReturnNull(string birthDate, string birthPlace, string description, string firstName, string lastName)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var newPerson = new AdminPersonModel
            {
                ID = 1,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actualPerson = await appPerson.Create(newPerson);
            #endregion

            #region Assert
            Assert.Null(actualPerson);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var person = new Domain.Person
            {
                BirthDate = "04-10-2010",
                BirthPlace = "Birth Place",
                Description = "Description",
                FirstName = "First Name",
                LastName = "Last Name"
            };
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            var expectedPerson = new PersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName
            };

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actualPerson = await appPerson.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedPerson.ID, actualPerson.ID);
            Assert.Equal(expectedPerson.BirthDate, actualPerson.BirthDate);
            Assert.Equal(expectedPerson.BirthPlace, actualPerson.BirthPlace);
            Assert.Equal(expectedPerson.Description, actualPerson.Description);
            Assert.Equal(expectedPerson.FirstName, actualPerson.FirstName);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_InvalidInput_ReturnsNull(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actualPerson = await appPerson.Read(id);
            #endregion

            #region Assert
            Assert.Null(actualPerson);
            #endregion
        }

        [Fact]
        public async Task ReadAll_PersonsExist_ReturnsAllPersons()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 2;

            dbContext.Persons.AddRange(
                Enumerable.Range(1, expectedAmount).Select(x => new Domain.Person
                {
                    ID = x,
                    BirthDate = "04-10-2010",
                    BirthPlace = $"Birth Place {x}",
                    Description = $"Description {x}",
                    FirstName = $"First Name {x}",
                    LastName = $"Last Name {x}"
                })
            );

            await dbContext.SaveChangesAsync();

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var result = await appPerson.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<PersonModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoPersonsExist_ReturnsEmptyList()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 0;

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var result = await appPerson.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<PersonModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Theory]
        [InlineData(1, "10-10-2010", "New Birth Place", "New Description", "New First Name", "New Last Name")]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string birthDate, string birthPlace, string description, string firstName, string lastName)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var person = new Domain.Person
            {
                BirthDate = "04-10-2010",
                BirthPlace = "Birth Place",
                Description = "Description",
                FirstName = "Firt Name",
                LastName = "Last Name"
            };
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            var newPerson = new AdminPersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var expectedPerson = new PersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actualPerson = await appPerson.Update(newPerson);
            #endregion

            #region Assert
            Assert.Equal(expectedPerson.ID, actualPerson.ID);
            Assert.Equal(expectedPerson.BirthDate, actualPerson.BirthDate);
            Assert.Equal(expectedPerson.BirthPlace, actualPerson.BirthPlace);
            Assert.Equal(expectedPerson.Description, actualPerson.Description);
            Assert.Equal(expectedPerson.FirstName, actualPerson.FirstName);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidInput_ReturnsNull()
        {
            int id = 1;
            var birthDate = "10-10-2010";
            var birthPlace = "New Birth Place";
            var descripton = "New Description";
            var firstName = "New First Name";
            var lastName = "New Last Name";

            // id = 2 (does not exist)
            yield return new object[] { 2, birthDate, birthPlace, descripton, firstName, lastName };
            // birthDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] { id, "1-1-0001 00:00:00", birthPlace, descripton, firstName, lastName };
            // birthPlace = null
            yield return new object[] { id, birthDate, null, descripton, firstName, lastName };
            // birthPlace = empty
            yield return new object[] { id, birthDate, "", descripton, firstName, lastName };
            // descripton = null
            yield return new object[] { id, birthDate, birthPlace, null, firstName, lastName };
            // descripton = empty
            yield return new object[] { id, birthDate, birthPlace, "", firstName, lastName };
            // firstName = null
            yield return new object[] { id, birthDate, birthPlace, descripton, null, lastName };
            // firstName = empty
            yield return new object[] { id, birthDate, birthPlace, descripton, "", lastName };
            // lastName = null
            yield return new object[] { id, birthDate, birthPlace, descripton, firstName, null };
            // lastName = empty
            yield return new object[] { id, birthDate, birthPlace, descripton, firstName, "" };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidInput_ReturnsNull))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string birthDate, string birthPlace, string description, string firstName, string lastName)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var person = new Domain.Person
            {
                BirthDate = "04-10-2010",
                BirthPlace = "Description",
                Description = "Description",
                FirstName = "First Name",
                LastName = "Last Name"
            };
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            var newPerson = new AdminPersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actualPerson = await appPerson.Update(newPerson);
            #endregion

            #region Assert
            Assert.Null(actualPerson);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            dbContext.Persons.Add(new Domain.Person());
            await dbContext.SaveChangesAsync();

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actual = await appPerson.Delete(id);
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

            var appPerson = new Person(dbContext);
            #endregion

            #region Act
            var actual = await appPerson.Delete(id);
            #endregion

            #region Assert
            Assert.False(actual);
            #endregion
        }
    }
}
