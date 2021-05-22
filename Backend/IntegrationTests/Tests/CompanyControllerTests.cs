using API;
using Application;
using Domain.Enums;
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
        [InlineData("Name", CompanyTypes.Distributor)]
        public async Task Create_ValidInput_ReturnsJsonResponseAndCreated(string name, CompanyTypes companyType)
        {
            #region Arrange 
            var dbContent = CreateDbContext();

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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCompany.ID, actualCompany.ID);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.Type, actualCompany.Type);
            #endregion
        }
    }
}
