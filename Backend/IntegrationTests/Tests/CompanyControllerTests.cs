using API;
using Application.AdminModels;
using Application.ViewModels;
using Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    [Collection("Sequential")]
    public class CompanyControllerTests : IntegrationTestSetup
    {
        public CompanyControllerTests(ApiFactory<Startup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("Name", CompanyTypes.Producer)]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string name, CompanyTypes companyType)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newCompany = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };

            var expectedCompany = new CompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType.ToString()
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/company", newCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<CompanyModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            string newName = "Name";
            CompanyTypes newCompanyType = CompanyTypes.Producer;

            // Name = null
            yield return new object[]
            {
                null, newCompanyType,
                new string[]
                {
                    "Name"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // Name = empty
            yield return new object[]
            {
                "", newCompanyType,
                new string[]
                {
                    "Name"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // CompanyType = 100
            yield return new object[]
            {
                newName, 100,
                new string[]
                {
                    "Type"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // Everything wrong
            yield return new object[]
            {
                null, 100,
                new string[]
                {
                    "Name",
                    "Type"
                },
                new string[]
                {
                    "Cannot be null or empty.",
                    "Does not exist."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(string name, CompanyTypes companyType, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newCompany = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/company", newCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorValues, errorValue => Assert.Contains(errorValue, errors.Select(prop => prop.Value[0].ToString())));
            #endregion
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task ConnectMovie_ValidRequest_ReturnsJsonResponseAndOk(int id, int movieID)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var company = new Domain.Company
            {
                Name = "Epic Producer",
                Type = CompanyTypes.Producer
            };
            dbContext.Companies.Add(company);

            var movie = new Domain.Movie
            {
                Title = "Epic Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var expectedCompany = new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
                Movies = new List<MovieModel>
                {
                    new MovieModel
                    {
                        ID = movie.ID,
                        Title = movie.Title
                    }
                }
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync($"/api/company/{id}/movies", movieID);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<CompanyModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Movies.Count(), actualCompany.Movies.Count());
            Assert.Equal(expectedCompany.Movies.ToList()[0].ID, actualCompany.Movies.ToList()[0].ID);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, new string[] { "CompanyID", "MovieID" }, new string[] { "Does not exist.", "Does not exist." })]
        [InlineData(0, 1, new string[] { "CompanyID" }, new string[] { "Does not exist." })]
        [InlineData(1, 0, new string[] { "MovieID" }, new string[] { "Does not exist." })]
        [InlineData(1, 1, new string[] { "CompanyMovieID" }, new string[] { "Does already exist." })]
        public async Task ConnectMovie_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors(int id, int movieID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company());
            dbContext.Movies.Add(new Domain.Movie());

            if (id == 1 && movieID == 1)
            {
                dbContext.CompanyMovies.Add(new Domain.CompanyMovie
                {
                    CompanyID = id,
                    MovieID = movieID
                });
            }

            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync($"/api/company/{id}/movies", movieID);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorMessages, errorMessage => Assert.Contains(errorMessage, errors.Select(prop => prop.Value[0].ToString())));
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidRequest_ReturnsJsonResponseAndOk(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Producer
            });
            await dbContext.SaveChangesAsync();

            var expectedCompany = new CompanyModel
            {
                ID = 1,
                Name = "Name",
                Type = CompanyTypes.Producer.ToString()
            };
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/company/{id}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<CompanyModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task Read_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Producer
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/company/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Fact]
        public async Task ReadAll_CompaniesExist_ReturnsJsonResponseAndOk()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Producer
            });
            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Some other company",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();

            int expectedCompanyCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/company");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompanies = await JsonSerializer.DeserializeAsync<IEnumerable<CompanyModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualCompanies);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCompanyCount, actualCompanies.Count());
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoCompaniesExist_ReturnsJsonResponseAndNoContent()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync("/api/company");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "Some other name", CompanyTypes.Producer)]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string name, CompanyTypes companyType)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
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
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/company/{newCompany.ID}", newCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<CompanyModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            int id = 1;
            string newName = "Some other name";
            CompanyTypes newCompanyType = CompanyTypes.Producer;

            // ID = 0
            yield return new object[] 
            { 
                0, newName, newCompanyType, 
                new string[] 
                { 
                    "ID" 
                },  
                new string[]
                {
                    "Must be above 0."
                }
            };
            // Name = null
            yield return new object[] 
            {
                id, null, newCompanyType, 
                new string[] 
                {
                    "Name" 
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // Name = empty
            yield return new object[] 
            { 
                id, "", newCompanyType, 
                new string[] 
                { 
                    "Name" 
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // CompanyType = 100
            yield return new object[] 
            { 
                id, newName, 100, 
                new string[]
                { 
                    "Type" 
                }, 
                new string[]
                {
                    "Does not exist."
                }
            };
            // Everything wrong
            yield return new object[] 
            { 
                0, null, 100, 
                new string[] 
                { 
                    "ID", 
                    "Name", 
                    "Type" 
                }, 
                new string[]
                {
                    "Must be above 0.",
                    "Cannot be null or empty.",
                    "Does not exist."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(int id, string name, CompanyTypes companyType, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();

            var newCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/company/{id}", newCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorMessages, errorMessage => Assert.Contains(errorMessage, errors.Select(prop => prop.Value[0].ToString())));
            #endregion
        }

        [Theory]
        [InlineData(2)]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();

            var newCompany = new AdminCompanyModel
            {
                ID = id,
                Name = "Some other name",
                Type = CompanyTypes.Producer
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/company/{id}", newCompany);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_ValidRequest_ReturnsJsonResponseAndOk(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/company/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task Delete_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/company/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task DisconnectMovie_ValidRequest_ReturnsJsonResponseAndOk(int id, int movieID)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var company = new Domain.Company
            {
                Name = "Epic Producer",
                Type = CompanyTypes.Producer
            };
            dbContext.Companies.Add(company);

            var movie = new Domain.Movie
            {
                Title = "Epic Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            dbContext.CompanyMovies.Add(new Domain.CompanyMovie
            {
                CompanyID = company.ID,
                MovieID = movie.ID
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/company/{id}/movies/{movieID}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, new string[] { "CompanyID", "MovieID" }, new string[] { "Does not exist.", "Does not exist." })]
        [InlineData(0, 1, new string[] { "CompanyID" }, new string[] { "Does not exist." })]
        [InlineData(1, 0, new string[] { "MovieID" }, new string[] { "Does not exist." })]
        [InlineData(1, 1, new string[] { "CompanyMovieID" }, new string[] { "Does not exist." })]
        public async Task DisconnectMovie_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors(int id, int movieID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var company = new Domain.Company();
            dbContext.Companies.Add(company);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            if (id != 1 && movieID != 1)
            {
                dbContext.CompanyMovies.Add(new Domain.CompanyMovie
                {
                    CompanyID = company.ID,
                    MovieID = movie.ID
                });
            }
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/company/{id}/movies/{movieID}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorMessages, errorMessage => Assert.Contains(errorMessage, errors.Select(prop => prop.Value[0].ToString())));
            #endregion
        }
    }
}
