using API;
using Application;
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
            : base(factory)
        { }

        [Theory]
        [InlineData("Name", CompanyTypes.Producer)]
        public async Task Create_ValidInput_ReturnsJsonResponseAndCreated(string name, CompanyTypes companyType)
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
            var response = await client.PostAsJsonAsync("/company/create", expectedCompany);
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

        public static IEnumerable<object[]> CreateInvalidInputData()
        {
            string name = "Name";
            CompanyTypes companyType = CompanyTypes.Producer;

            // name = null | companyType = 100 (does not exists)
            yield return new object[] { null, 100, new string[] { "Name", "CompanyType" } };
            // name = null
            yield return new object[] { null, companyType, new string[] { "Name" } };
            // name = empty
            yield return new object[] { "", companyType, new string[] { "Name" } };
            // companyType = 100 (does not exists)
            yield return new object[] { name, 100, new string[] { "CompanyType" } };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidInputData))]
        public async Task Create_InvalidInput_ReturnsJsonResponseAndBadRequest(string name, CompanyTypes companyType, string[] expectedErrors)
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
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/company/create", invalidCompanyData);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCompany = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);
            #endregion

            #region Assert
            var errorProp = actualCompany.GetProperty("errors");
            var errors = errorProp.EnumerateObject();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedErrors.Length, errors.Count());
            #endregion
        }
    }
}
