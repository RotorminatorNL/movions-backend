using API;
using Application.AdminModels;
using Application.ViewModels;
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
    public class LanguageControllerTests : IntegrationTestSetup
    {
        public LanguageControllerTests(ApiFactory<Startup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("Name")]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string name)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newLanguage = new AdminLanguageModel
            {
                Name = name
            };

            var expectedLanguage = new LanguageModel
            {
                ID = 1,
                Name = "Name"
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/language", newLanguage);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualLanguage = await JsonSerializer.DeserializeAsync<LanguageModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
            Assert.Equal(expectedLanguage.Name, actualLanguage.Name);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            // Name = null
            yield return new object[]
            {
                null,
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
                "",
                new string[]
                {
                    "Name"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(string name, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newLanguage = new AdminLanguageModel
            {
                Name = name
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/language", newLanguage);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualLanguage = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualLanguage.GetProperty("errors");
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
        [InlineData(1)]
        public async Task Read_ValidRequest_ReturnsJsonResponseAndOk(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var expectedLanguage = new LanguageModel
            {
                ID = 1,
                Name = "Name"
            };
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/language/{id}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualLanguage = await JsonSerializer.DeserializeAsync<LanguageModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
            Assert.Equal(expectedLanguage.Name, actualLanguage.Name);
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

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/language/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Fact]
        public async Task ReadAll_LanguagesExist_ReturnsJsonResponseAndOk()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Some other name"
            });
            await dbContext.SaveChangesAsync();

            int expectedLanguageCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/language");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualLanguages = await JsonSerializer.DeserializeAsync<IEnumerable<LanguageModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualLanguages);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedLanguageCount, actualLanguages.Count());
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoLanguagesExist_ReturnsJsonResponseAndNoContent()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync("/api/language");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "Some other name")]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string name)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var newLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = name
            };

            var expectedLanguage = new LanguageModel
            {
                ID = 1,
                Name = "Some other name"
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/language/{newLanguage.ID}", newLanguage);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualLanguage = await JsonSerializer.DeserializeAsync<LanguageModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedLanguage.ID, actualLanguage.ID);
            Assert.Equal(expectedLanguage.Name, actualLanguage.Name);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            int id = 1;

            // Name = null
            yield return new object[]
            {
                id, null,
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
                id, "",
                new string[]
                {
                    "Name"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // Both wrong
            yield return new object[]
            {
                0, null,
                new string[]
                {
                    "Name"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(int id, string name, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var newLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = name
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/language/{id}", newLanguage);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualLanguage = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualLanguage.GetProperty("errors");
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

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var newLanguage = new AdminLanguageModel
            {
                ID = id,
                Name = "Some other name"
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/language/{id}", newLanguage);
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

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/language/{id}");
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

            dbContext.Languages.Add(new Domain.Language
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/language/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }
    }
}
