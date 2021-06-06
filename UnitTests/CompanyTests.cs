using Application;
using Application.AdminModels;
using Application.ViewModels;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var newCompany = new AdminCompanyModel
            {
                Name = name,
                Type = companyType
            };

            var expectedCompany = new CompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType.ToString()
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Create(newCompany);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidInput_ReturnNull()
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
        [MemberData(nameof(Data_Create_InvalidInput_ReturnNull))]
        public async Task Create_InvalidInput_ReturnNull(string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.SaveChangesAsync();

            var newCompany = new AdminCompanyModel
            {
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion
            
            #region Act
            var actualCompany = await appCompany.Create(newCompany);
            #endregion

            #region Assert
            Assert.Null(actualCompany);
            #endregion
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task ConnectMovie_ValidInput_ReturnsCorrectData(int id, int movieID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            dbContext.Companies.Add(new Domain.Company());
            dbContext.Movies.Add(new Domain.Movie());
            await dbContext.SaveChangesAsync();

            var newCompanyMovie = new AdminCompanyMovieModel
            {
                CompanyID = id,
                MovieID = movieID
            };

            var expectedCompany = new CompanyModel
            {
                ID = 1,
                Movies = new List<MovieModel>
                {
                    new MovieModel
                    {
                        ID = 1,
                    }
                }
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.ConnectMovie(newCompanyMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Movies.Count(), actualCompany.Movies.Count());
            Assert.Equal(expectedCompany.Movies.ToList()[0].ID, actualCompany.Movies.ToList()[0].ID);
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
        public async Task ConnectMovie_InvalidInput_ReturnsNull(int id, int movieID, int expectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company();
            dbContext.Companies.Add(company);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            if (expectedID == -4)
            {
                dbContext.CompanyMovies.Add(new Domain.CompanyMovie
                {
                    CompanyID = company.ID,
                    MovieID = movie.ID
                });
            }

            await dbContext.SaveChangesAsync();

            var newCompanyMovie = new AdminCompanyMovieModel
            {
                CompanyID = id,
                MovieID = movieID
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.ConnectMovie(newCompanyMovie);
            #endregion

            #region Assert
            Assert.NotNull(actualCompany);
            Assert.Equal(expectedID, actualCompany.ID);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidInput_ReturnsCorrectData(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company
            {
                ID = id,
                Name = "Name",
                Type = CompanyTypes.Distributor
            };
            dbContext.Companies.Add(company);
            await dbContext.SaveChangesAsync();

            var expectedCompany = new CompanyModel
            {
                ID = id,
                Name = company.Name,
                Type = company.Type.ToString()
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_InvalidInput_ReturnsNull(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Read(id);
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

            int expectedAmount = 2;

            dbContext.Companies.AddRange(
                Enumerable.Range(1, expectedAmount).Select(x => new Domain.Company
                {
                    ID = x,
                    Name = $"Name {x}",
                    Type = CompanyTypes.Producer
                })
            );

            await dbContext.SaveChangesAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var result = await appCompany.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<CompanyModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoCompaniesExist_ReturnsEmptyList()
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            int expectedAmount = 0;

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var result = await appCompany.ReadAll();
            #endregion

            #region Assert
            var actualAmount = Assert.IsAssignableFrom<IEnumerable<CompanyModel>>(result).Count();
            Assert.Equal(expectedAmount, actualAmount);
            #endregion
        }

        [Theory]
        [InlineData(1, "New Name", CompanyTypes.Distributor)]
        public async Task Update_ValidInput_ReturnsCorrectData(int id, string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Producer
            };
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var newCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };

            var expectedCompany = new CompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType.ToString()
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Update(newCompany);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidInput_ReturnsNull()
        {
            int id = 1;
            string name = "New Name";
            CompanyTypes companyType = CompanyTypes.Distributor;

            // id = 2 (does not exist)
            yield return new object[] { 2, name, companyType };
            // name = null
            yield return new object[] { id, null, companyType };
            // name = empty
            yield return new object[] { id, "", companyType };
            // companyType = 100 (does not exist)
            yield return new object[] { id, name, 100 };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidInput_ReturnsNull))]
        public async Task Update_InvalidInput_ReturnsNull(int id, string name, CompanyTypes companyType)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Producer
            };
            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            var newCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.Update(newCompany);
            #endregion

            #region Assert
            Assert.Null(actualCompany);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidInput_ReturnsTrue(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            dbContext.Companies.Add(new Domain.Company());
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
        [InlineData(1)]
        public async Task Delete_InvalidInput_ReturnsFalse(int id)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actual = await appCompany.Delete(id);
            #endregion

            #region Assert
            Assert.False(actual);
            #endregion
        }

        [Theory]
        [InlineData(1, 1, 0)]
        public async Task DisconnectMovie_ValidInput_ReturnsTrue(int id, int movieID, int expectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company();
            dbContext.Companies.Add(company);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            var companyMovie = new Domain.CompanyMovie
            {
                CompanyID = company.ID,
                MovieID = movie.ID
            };
            dbContext.CompanyMovies.Add(companyMovie);

            await dbContext.SaveChangesAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actual = await appCompany.DisconnectMovie(new AdminCompanyMovieModel { CompanyID = id, MovieID = movieID });
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
        public async Task DisconnectMovie_InvalidInput_ReturnsCompanyModelWithErrorID(int id, int movieID, int exptectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var company = new Domain.Company();
            dbContext.Companies.Add(company);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            if (exptectedID != -4)
            {
                var companyMovie = new Domain.CompanyMovie
                {
                    CompanyID = company.ID,
                    MovieID = movie.ID
                };
                dbContext.CompanyMovies.Add(companyMovie);
            }

            await dbContext.SaveChangesAsync();

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actual = await appCompany.DisconnectMovie(new AdminCompanyMovieModel { CompanyID = id, MovieID = movieID });
            #endregion

            #region Assert
            Assert.NotNull(actual);
            Assert.Equal(exptectedID, actual.ID);
            #endregion
        }
    }
}
