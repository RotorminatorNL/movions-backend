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
    }
}
