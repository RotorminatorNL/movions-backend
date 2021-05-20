using Application;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class CompanyTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public CompanyTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_company")
                .Options;
        }

        [Theory]
        [InlineData("Name", CompanyTypes.Distributor)]
        public async Task Create_ValidInput_ReturnsCorrectData(string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Create(expectedCompany);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidInputData()
        {
            string name = "Name";
            CompanyTypes companyType = CompanyTypes.Distributor;

            // name = null
            yield return new object[] { null, companyType };
            // name = empty
            yield return new object[] { "", companyType };
            // companyType = 100 (does not exists)
            yield return new object[] { name, 100 };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidInputData))]
        public async Task Create_InvalidInput_ReturnNull(string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion
            
            #region Act
            var actualCompany = await appCompany.Create(expectedCompany);
            #endregion

            #region Assert
            Assert.Null(actualCompany);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = "Name",
                Type = CompanyTypes.Distributor
            };

            var appCompany = new Company(dbContext);

            await appCompany.Create(expectedCompany);
            #endregion

            #region Act
            var actualCompany = appCompany.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        public async Task Read_InvalidInput_ReturnsNull(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = "Name",
                Type = CompanyTypes.Distributor
            };

            var appCompany = new Company(dbContext);

            await appCompany.Create(expectedCompany);
            #endregion

            #region Act
            var actualCompany = appCompany.Read(id);
            #endregion

            #region Assert
            Assert.Null(actualCompany);
            #endregion
        }

        [Fact]
        public async Task ReadAll_CompaniesExist_ReturnsAllCompanies()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 5;

            dbContext.Companies.AddRange(
                Enumerable.Range(1, expectedAmount).Select(c => new Domain.Company
                {
                    ID = c,
                    Name = $"Name {c}",
                    Type = CompanyTypes.Producer
                })
            );

            await dbContext.SaveChangesAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var result = appCompany.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<CompanyModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoCompaniesExist_ReturnsAllCompanies()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 0;

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var result = appCompany.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<CompanyModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Theory]
        [InlineData(1, "Name", CompanyTypes.Distributor)]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company
            {
                Name = "Test",
                Type = CompanyTypes.Producer
            };
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Update(expectedCompany);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> UpdateInvalidInputData()
        {
            int id = 1;
            string name = "Name";
            CompanyTypes companyType = CompanyTypes.Distributor;

            // id = 0
            yield return new object[] { 0, name, companyType };
            // name = null
            yield return new object[] { id, null, companyType };
            // name = empty
            yield return new object[] { id, "", companyType };
            // companyType = 100 (does not exist)
            yield return new object[] { id, name, 100 };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidInputData))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company
            {
                Name = "Test",
                Type = CompanyTypes.Producer
            };
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Update(expectedCompany);
            #endregion

            #region Assert
            Assert.Null(actualCompany);
            #endregion
        }

        [Theory]
        [InlineData(1, "Name", CompanyTypes.Distributor)]
        public async Task Update_InputIsNotDifferent_ReturnsEmptyAdminCompanyModel(int id, string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company
            {
                Name = name,
                Type = companyType
            };
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var newCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };

            var expectedCompany = new AdminCompanyModel();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Update(newCompany);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company();
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actual = await appCompany.Delete(id);
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

            var company = new Domain.Company();
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actual = await appCompany.Delete(id);
            #endregion

            #region Assert
            Assert.False(actual);
            #endregion
        }
    }
}
