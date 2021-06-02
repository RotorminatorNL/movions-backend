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

            var adminCompanyModel = new AdminCompanyModel
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
            var actualCompany = await appCompany.Create(adminCompanyModel);
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

            var adminCompanyModel = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };

            var appCompany = new Company(dbContext);
            #endregion
            
            #region Act
            var actualCompany = await appCompany.Create(adminCompanyModel);
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
            var actualCompany = await appCompany.Read(id);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
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

        public static IEnumerable<object[]> UpdateInvalidInputData()
        {
            int id = 1;
            string name = "Name";
            CompanyTypes companyType = CompanyTypes.Distributor;

            // id = 0
            yield return new object[] { 0, name, companyType };
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
        [InlineData(1, 1)]
        public async Task ConnectMovie_ValidInput_ReturnsCorrectData(int id, int movieID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var newCompany = new Domain.Company
            {
                Name = "Epic Producer",
                Type = CompanyTypes.Producer
            };

            var newMovie = new Domain.Movie
            {
                Title = "Epic Title"
            };

            dbContext.Companies.Add(newCompany);
            dbContext.Movies.Add(newMovie);
            await dbContext.SaveChangesAsync();

            var companyMovie = new AdminCompanyMovieModel
            {
                CompanyID = id,
                MovieID = movieID
            };

            var expectedCompany = new CompanyModel
            {
                ID = 1,
                Name = "Epic Producer",
                Type = CompanyTypes.Producer.ToString(),
                Movies = new List<MovieModel>
                {
                    new MovieModel
                    {
                        ID = 1,
                        Title = "Epic Title"
                    }
                }
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.ConnectMovie(companyMovie);
            #endregion

            #region Assert
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            Assert.Equal(expectedCompany.Movies.Count(), actualCompany.Movies.Count());
            Assert.Equal(expectedCompany.Movies.ToList()[0].ID, actualCompany.Movies.ToList()[0].ID);
            Assert.Equal(expectedCompany.Movies.ToList()[0].Title, actualCompany.Movies.ToList()[0].Title);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, -3)]
        [InlineData(0, 1, -2)]
        [InlineData(1, 0, -1)]
        [InlineData(1, 2, -1)]
        [InlineData(2, 1, -2)]
        [InlineData(2, 2, -3)]
        public async Task ConnectMovie_InvalidInput_ReturnsNull(int id, int movieID, int expectedID)
        {
            #region Arrange
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            await dbContext.Database.EnsureDeletedAsync();

            var newCompany = new Domain.Company
            {
                Name = "Epic Producer",
                Type = CompanyTypes.Producer
            };

            var newMovie = new Domain.Movie
            {
                Title = "Epic Title"
            };

            dbContext.Companies.Add(newCompany);
            dbContext.Movies.Add(newMovie);
            await dbContext.SaveChangesAsync();

            var companyMovie = new AdminCompanyMovieModel
            {
                CompanyID = id,
                MovieID = movieID
            };

            var appCompany = new Company(dbContext);
            #endregion

            #region Act
            var actualCompany = await appCompany.ConnectMovie(companyMovie);
            #endregion

            #region Assert
            Assert.NotNull(actualCompany);
            Assert.Equal(expectedID, actualCompany.ID);
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

            var language = new Domain.Language();
            dbContext.Languages.Add(language);

            var movie = new Domain.Movie { LanguageID = language.ID };
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
            Assert.Equal(exptectedID, actual.ID);
            #endregion
        }
    }
}
