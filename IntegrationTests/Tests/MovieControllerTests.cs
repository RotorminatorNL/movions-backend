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
            : base(factory) 
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");
        }

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
                Name = title
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
                Name = title
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
            Assert.Equal(expectedMovie.Name, actualMovie.Name);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            string description = "Description";
            int languageID = 1;
            int length = 104;
            string releaseDate = "04-10-2010";
            string title = "Title";

            // Description = null
            yield return new object[]
            {
                null, languageID, length, releaseDate, title,
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
                "", languageID, length, releaseDate, title,
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
                description, 0, length, releaseDate, title,
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
                description, languageID, 0, releaseDate, title,
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
                description, languageID, length, "1-1-0001 00:00:00", title,
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
                description, languageID, length, releaseDate, null,
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
                description, languageID, length, releaseDate, "",
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
                    "ReleaseDate",
                    "Title"

                },
                new string[]
                {
                    "Cannot be null or empty.",
                    "Must be above 0.",
                    "Must be above 0.",
                    "Must be later than 1-1-0001 00:00:00.",
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
                Name = title
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/movie", newMovie);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualMovie.GetProperty("errors");
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
        [InlineData("Description", 2, 104, "04-10-2010", "Title", new string[] { "LanguageID" }, new string[] { "Does not exist." })]
        public async Task Create_LanguageDoesNotExist_ReturnsJsonResponseAndNotFoundWithErrors(string description, int languageID, int length, string releaseDate, string title, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
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
                Name = title
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/movie", newMovie);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualMovie.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorValues, errorValue => Assert.Contains(errorValue, errors.Select(prop => prop.Value[0].ToString())));
            #endregion
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task ConnectGenre_ValidRequest_ReturnsJsonResponseAndOk(int id, int genreID)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var language = new Domain.Language();
            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            dbContext.Genres.Add(new Domain.Genre());
            dbContext.Movies.Add(new Domain.Movie 
            { 
                LanguageID = language.ID,
                ReleaseDate = "04-10-2010"
            });
            await dbContext.SaveChangesAsync();

            var expectedMovie = new MovieModel
            {
                ID = id,
                Genres = new List<GenreModel>
                {
                    new GenreModel
                    {
                        ID = genreID
                    }
                }
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync($"/api/movie/{id}/genres", genreID);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<MovieModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Genres.Count(), actualMovie.Genres.Count());
            Assert.Equal(expectedMovie.Genres.ToList()[0].ID, actualMovie.Genres.ToList()[0].ID);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, new string[] { "GenreID", "MovieID" }, new string[] { "Does not exist.", "Does not exist." })]
        [InlineData(0, 1, new string[] { "MovieID" }, new string[] { "Does not exist." })]
        [InlineData(1, 0, new string[] { "GenreID" }, new string[] { "Does not exist." })]
        [InlineData(1, 1, new string[] { "GenreMovieID" }, new string[] { "Does already exist." })]
        public async Task ConnectGenre_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors(int id, int genreID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Genres.Add(new Domain.Genre());
            dbContext.Movies.Add(new Domain.Movie());

            if (id == 1 && genreID == 1)
            {
                dbContext.GenreMovies.Add(new Domain.GenreMovie
                {
                    GenreID = genreID,
                    MovieID = id
                });
            }

            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync($"/api/movie/{id}/genres", genreID);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualMovie.GetProperty("errors");
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

            var language = new Domain.Language
            {
                Name = "Name"
            };
            dbContext.Languages.Add(language);
            await dbContext.SaveChangesAsync();

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = 1,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Name = "Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var expectedMovie = new MovieModel
            {
                ID = 1,
                Description = movie.Description,
                Language = new LanguageModel
                {
                    ID = language.ID,
                    Name = language.Name
                },
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Name = movie.Name
            };
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/movie/{id}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<MovieModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Language.Name, actualMovie.Language.Name);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Name, actualMovie.Name);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/movie/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Fact]
        public async Task ReadAll_MoviesExist_ReturnsJsonResponseAndOk()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var langauge = new Domain.Language();
            dbContext.Languages.Add(langauge);
            await dbContext.SaveChangesAsync();

            dbContext.Movies.Add(new Domain.Movie
            {
                LanguageID = langauge.ID,
                ReleaseDate = "04-10-2010"
            });
            dbContext.Movies.Add(new Domain.Movie
            {
                LanguageID = langauge.ID,
                ReleaseDate = "04-10-2010"
            });
            await dbContext.SaveChangesAsync();

            int expectedCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/movie");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovies = await JsonSerializer.DeserializeAsync<IEnumerable<MovieModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualMovies);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCount, actualMovies.Count());
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoMoviesExist_ReturnsJsonResponseAndNoContent()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync("/api/movie");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "New Description", 2, 110, "10-10-2010", "New Title")]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string description, int languageID, int length, string releaseDate, string title)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var language = new Domain.Language { Name = "Name" };
            dbContext.Languages.Add(language);
            var language2 = new Domain.Language { Name = "New Name" };
            dbContext.Languages.Add(language2);
            await dbContext.SaveChangesAsync();

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = language.ID,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Name = "Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Name = title
            };

            var expectedMovie = new MovieModel
            {
                ID = id,
                Description = description,
                Language = new LanguageModel
                {
                    ID = language2.ID,
                    Name = language2.Name
                },
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Name = title
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/movie/{id}", newMovie);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<MovieModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMovie.ID, actualMovie.ID);
            Assert.Equal(expectedMovie.Description, actualMovie.Description);
            Assert.Equal(expectedMovie.Language.ID, actualMovie.Language.ID);
            Assert.Equal(expectedMovie.Language.Name, actualMovie.Language.Name);
            Assert.Equal(expectedMovie.Length, actualMovie.Length);
            Assert.Equal(expectedMovie.ReleaseDate, actualMovie.ReleaseDate);
            Assert.Equal(expectedMovie.Name, actualMovie.Name);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            int id = 1;
            string description = "New Description";
            int languageID = 2;
            int length = 110;
            string releaseDate = "10-10-2010";
            string title = "New Title";

            // Description = null
            yield return new object[]
            {
                id, null, languageID, length, releaseDate, title,
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
                id, "", languageID, length, releaseDate, title,
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
                id, description, 0, length, releaseDate, title,
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
                id, description, languageID, 0, releaseDate, title,
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
                id, description, languageID, length, "1-1-0001 00:00:00", title,
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
                id, description, languageID, length, releaseDate, null,
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
                id, description, languageID, length, releaseDate, "",
                new string[]
                {
                    "Title"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // everything = wrong
            yield return new object[]
            {
                id, null, 0, 0, "1-1-0001 00:00:00", null,
                new string[]
                {
                    "Description",
                    "LanguageID",
                    "Length",
                    "ReleaseDate",
                    "Title"
                },
                new string[]
                {
                    "Cannot be null or empty.",
                    "Must be above 0.",
                    "Must be above 0.",
                    "Must be later than 1-1-0001 00:00:00.",
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(int id, string description, int languageID, int length, string releaseDate, string title, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var language = new Domain.Language { Name = "Name" };
            dbContext.Languages.Add(language);
            dbContext.Languages.Add(new Domain.Language { Name = "New Name" });
            await dbContext.SaveChangesAsync();

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = language.ID,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Name = "Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Name = title
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/movie/{id}", newMovie);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualMovie.GetProperty("errors");
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
        [InlineData(1)]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newMovie = new AdminMovieModel
            {
                ID = id,
                Description = "Description",
                LanguageID = 1,
                Length = 104,
                ReleaseDate = DateTime.Parse("04-10-2010"),
                Name = "Title"
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/movie/{id}", newMovie);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "Description", 2, 104, "04-10-2010", "Title", new string[] { "LanguageID" }, new string[] { "Does not exist." })]
        public async Task Update_LanguageDoesNotExist_ReturnsJsonResponseAndNotFoundWithErrors(int id, string description, int languageID, int length, string releaseDate, string title, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
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

            var movie = new Domain.Movie
            {
                Description = "Description",
                LanguageID = language.ID,
                Length = 104,
                ReleaseDate = "04-10-2010",
                Name = "Title"
            };
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            var newMovie = new AdminMovieModel
            {
                ID = id,
                Description = description,
                LanguageID = languageID,
                Length = length,
                ReleaseDate = DateTime.Parse(releaseDate),
                Name = title
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/movie", newMovie);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualMovie.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorValues, errorValue => Assert.Contains(errorValue, errors.Select(prop => prop.Value[0].ToString())));
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

            dbContext.Movies.Add(new Domain.Movie());
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/movie/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Delete_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/movie/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task DisconnectGenre_ValidRequest_ReturnsJsonResponseAndOk(int id, int genreID)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var genre = new Domain.Genre();
            dbContext.Genres.Add(genre);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            dbContext.GenreMovies.Add(new Domain.GenreMovie
            {
                GenreID = genre.ID,
                MovieID = movie.ID
            });
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/movie/{id}/genres/{genreID}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(0, 0, new string[] { "GenreID", "MovieID" }, new string[] { "Does not exist.", "Does not exist." })]
        [InlineData(0, 1, new string[] { "MovieID" }, new string[] { "Does not exist." })]
        [InlineData(1, 0, new string[] { "GenreID" }, new string[] { "Does not exist." })]
        [InlineData(1, 1, new string[] { "GenreMovieID" }, new string[] { "Does not exist." })]
        public async Task DisconnectGenre_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors(int id, int genreID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var genre = new Domain.Genre();
            dbContext.Genres.Add(genre);

            var movie = new Domain.Movie();
            dbContext.Movies.Add(movie);

            if (id != 1 && genreID != 1)
            {
                dbContext.GenreMovies.Add(new Domain.GenreMovie
                {
                    GenreID = genre.ID,
                    MovieID = movie.ID
                });
            }
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/movie/{id}/genres/{genreID}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualMovie = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualMovie.GetProperty("errors");
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
