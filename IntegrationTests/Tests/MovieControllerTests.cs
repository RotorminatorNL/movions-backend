using API;
using Application.AdminModels;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{   
    [Collection("Sequential")]
    public class MovieControllerTests : IntegrationTestSetup
    {
        public MovieControllerTests(ApiFactory<Startup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("Description", 1, 104, "04-10-2010", "Title")]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var language = new Domain.Language
            {
                Name = "Name"
            };
            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };

            var expectedMovie = new MovieModel
            {
                ID = 1,
                Description = description,
                Language = new LanguageModel
                {
                    ID = language.ID,
                    Name = language.Name
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/movie", newMovie);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<MovieModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Language.Name, actualMovie.Language.Name);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            string newDescription = "Description";
            int newLanguageID = 1;
            int newLength = 104;
            string newReleaseDate = "04-10-2010";
            string newTitle = "Title";

            // Description = null
            yield return new object[]
            {
                null, newLanguageID, newLength, newReleaseDate, newTitle,
                new string[]
                {
                    "Description"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // Description = empty
            yield return new object[]
            {
                "", newLanguageID, newLength, newReleaseDate, newTitle,
                new string[]
                {
                    "Description"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // LanguageID = 0
            yield return new object[]
            {
                newDescription, 0, newLength, newReleaseDate, newTitle,
                new string[]
                {
                    "LanguageID"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
            // Length = 0
            yield return new object[]
            {
                newDescription, newLanguageID, 0, newReleaseDate, newTitle,
                new string[]
                {
                    "Length"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
            // ReleaseDate = 1-1-0001 00:00:00
            yield return new object[]
            {
                newDescription, newLanguageID, newLength, "1-1-0001 00:00:00", newTitle,
                new string[]
                {
                    "ReleaseDate"
                },
                new string[]
                {
                    "Must be later than 1-1-0001 00:00:00."
                }
            };
            // Title = null
            yield return new object[]
            {
                newDescription, newLanguageID, newLength, newReleaseDate, null,
                new string[]
                {
                    "Title"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // Title = empty
            yield return new object[]
            {
                newDescription, newLanguageID, newLength, newReleaseDate, "",
                new string[]
                {
                    "Title"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // Everything wrong
            yield return new object[]
            {
                null, 0, 0, "1-1-0001 00:00:00", null,
                new string[]
                {
                    "Description",
                    "LanguageID",
                    "Length",
                    "Title"

                },
                new string[]
                {
                    "Cannot be null or empty.",
                    "Must be above 0.",
                    "Must be above 0.",
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(string description, int languageID, int length, string releaseDate, string title, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var language = new Domain.Language
            {
                Name = "Name"
            };
            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Title = title
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/movie", newMovie);
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
    }
}
