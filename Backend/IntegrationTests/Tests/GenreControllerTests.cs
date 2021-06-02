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
    public class GenreControllerTests : IntegrationTestSetup
    {
        public GenreControllerTests(ApiFactory<Startup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("Name")]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string name)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newGenre = new AdminGenreModel
            {
                ID = 1,
                Name = name
            };

            var expectedGenre = new GenreModel
            {
                ID = 1,
                Name = name
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/genre", newGenre);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualGenre = await JsonSerializer.DeserializeAsync<GenreModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedGenre.ID, actualGenre.ID);
            Assert.Equal(expectedGenre.Name, actualGenre.Name);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidRequestData()
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
        [MemberData(nameof(CreateInvalidRequestData))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequest(string name, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newGenre = new AdminGenreModel
            {
                ID = 1,
                Name = name
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/genre", newGenre);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualGenre = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualGenre.GetProperty("errors");
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var expectedGenre = new GenreModel
            {
                ID = 1,
                Name = "Name"
            };
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/genre/{id}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualGenre = await JsonSerializer.DeserializeAsync<GenreModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedGenre.ID, actualGenre.ID);
            Assert.Equal(expectedGenre.Name, actualGenre.Name);
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/genre/{id}");
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Some other company"
            });
            await dbContext.SaveChangesAsync();

            int expectedGenreCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/genre");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualGenres = await JsonSerializer.DeserializeAsync<IEnumerable<GenreModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualGenres);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedGenreCount, actualGenres.Count());
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
            var response = await client.GetAsync("/api/genre");
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var newGenre = new AdminGenreModel
            {
                ID = id,
                Name = name
            };

            var expectedGenre = new GenreModel
            {
                ID = id,
                Name = name
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/genre/{newGenre.ID}", newGenre);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualGenre = await JsonSerializer.DeserializeAsync<GenreModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedGenre.ID, actualGenre.ID);
            Assert.Equal(expectedGenre.Name, actualGenre.Name);
            #endregion
        }

        public static IEnumerable<object[]> UpdateInvalidRequestData()
        {
            int id = 1;
            string newName = "Some other name";

            // ID = 0
            yield return new object[]
            {
                0, newName,
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
                    "ID",
                    "Name"
                },
                new string[]
                {
                    "Must be above 0.",
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidRequestData))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id, string name, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var newGenre = new AdminGenreModel
            {
                ID = id,
                Name = name
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/genre/{id}", newGenre);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualGenre = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualGenre.GetProperty("errors");
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();

            var newGenre = new AdminGenreModel
            {
                ID = id,
                Name = "Some other name"
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/genre/{id}", newGenre);
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/genre/{id}");
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

            dbContext.Genres.Add(new Domain.Genre
            {
                Name = "Name"
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/genre/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }
    }
}
