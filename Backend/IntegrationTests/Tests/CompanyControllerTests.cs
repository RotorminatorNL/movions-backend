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

            var client = CreateHttpClient();

            var expectedCompany = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/company", expectedCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<AdminCompanyModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidRequestData()
        {
            string name = "Name";
            CompanyTypes companyType = CompanyTypes.Producer;

            // object = null
            yield return new object[] { "null", 0, new string[] { "" } };
            // object = wrong model
            yield return new object[] { "wrongModel", 0, new string[] { "$.type" } };
            // name = null | companyType = 100 (does not exists)
            yield return new object[] { null, 100, new string[] { "Name", "Type" } };
            // name = null
            yield return new object[] { null, companyType, new string[] { "Name" } };
            // name = empty
            yield return new object[] { "", companyType, new string[] { "Name" } };
            // companyType = 100 (does not exists)
            yield return new object[] { name, 100, new string[] { "Type" } };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidRequestData))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequest(string name, CompanyTypes companyType, IEnumerable<string> expectedErrors)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = CreateHttpClient();

            var invalidCompanyData = new AdminCompanyModel
            {
                ID = 1,
                Name = name,
                Type = companyType
            };
            invalidCompanyData = invalidCompanyData.Name == "null" ? null : invalidCompanyData;

            var wrongModel = new CompanyModel { Name = name, Type = companyType.ToString() };
            #endregion

            #region Act
            var response = name == "wrongModel" 
                ? await client.PostAsJsonAsync("/api/company", wrongModel) 
                : await client.PostAsJsonAsync("/api/company", invalidCompanyData);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedErrors.Count(), errors.Count());
            Assert.All(expectedErrors, error => Assert.Contains(error, errors.Select(prop => prop.Name)));
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_ValidRequest_ReturnsJsonResponseAndOk(int id)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = CreateHttpClient();
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
        public async Task Read_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = CreateHttpClient();
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

            var client = CreateHttpClient();
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

            var client = CreateHttpClient();
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

            var client = CreateHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/company/{expectedCompany.ID}", expectedCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<AdminCompanyModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }

        public static IEnumerable<object[]> UpdateInvalidRequestData()
        {
            int id = 1;
            string newName = "Some other name";
            CompanyTypes newCompanyType = CompanyTypes.Producer;

            // object = null
            yield return new object[] { 0, "null", 0, new string[] { "" } };
            // object = wrong
            yield return new object[] { 0, "wrongModel", 0, new string[] { "$.type" } };
            // name = null
            yield return new object[] { id, null, newCompanyType, new string[] { "Name" } };
            // name = empty
            yield return new object[] { id, "", newCompanyType, new string[] { "Name" } };
            // companyType = 100 (does not exist)
            yield return new object[] { id, newName, 100, new string[] { "Type" } };
            // name = null | companyType = 100 (does not exist)
            yield return new object[] { id, null, 100, new string[] { "Name", "Type" } };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidRequestData))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id, string name, CompanyTypes companyType, IEnumerable<string> expectedErrors)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = CreateHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = name,
                Type = companyType
            };
            expectedCompany = expectedCompany.Name == "null" ? null : expectedCompany;

            var wrongModel = new CompanyModel { ID = id, Name = name, Type = companyType.ToString() };
            #endregion

            #region Act
            var response = name == "wrongModel" 
                ? await client.PutAsJsonAsync($"/api/company/{id}", wrongModel)
                : await client.PutAsJsonAsync($"/api/company/{id}", expectedCompany);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedErrors.Count(), errors.Count());
            Assert.All(expectedErrors, error => Assert.Contains(error, errors.Select(prop => prop.Name)));
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = CreateHttpClient();
            var dbContext = GetDbContext();

            dbContext.Companies.Add(new Domain.Company
            {
                Name = "Name",
                Type = CompanyTypes.Distributor
            });
            await dbContext.SaveChangesAsync();

            var expectedCompany = new AdminCompanyModel
            {
                ID = id,
                Name = "Some other name",
                Type = CompanyTypes.Producer
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/company/{id}", expectedCompany);
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

            var client = CreateHttpClient();
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

            var client = CreateHttpClient();
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
    }
}
