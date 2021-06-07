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
    public class CrewMemberControllerTests : IntegrationTestSetup
    {
        private async Task<object[]> CreatePersonAndMovie()
        {
            var dbContent = GetDbContext();

            var movie = new Domain.Movie
            {
                Title = "Title"
            };

            var person = new Domain.Person
            {
                FirstName = "First Name",
                LastName = "Last Name"
            };

            dbContent.Movies.Add(movie);
            dbContent.Persons.Add(person);
            await dbContent.SaveChangesAsync();

            return new object[] { movie, person };
        }

        public CrewMemberControllerTests(ApiFactory<Startup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("Character Name", CrewRoles.Actor, 1, 1)]
        [InlineData(null, CrewRoles.Director, 1, 1)]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var data = await CreatePersonAndMovie();
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var newCrewMember = new AdminCrewMemberModel
            {
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var expectedCrewMember = new CrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole.ToString(),
                Movie = new MovieModel
                {
                    ID = movie.ID,
                    Title = movie.Title
                },
                Person = new PersonModel
                {
                    ID = person.ID,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                }
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/crewmember", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<CrewMemberModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Movie.Title, actualCrewMember.Movie.Title);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            string characterName = "Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;
            int movieID = 1;
            int personID = 1;

            // CharacterName = null
            yield return new object[]
            {
                null, crewRoleActor, movieID, personID,
                new string[]
                {
                    "CharacterName"
                },
                new string[]
                {
                    "This role must have a character name."
                }
            };
            // CharacterName = empty
            yield return new object[]
            {
                "", crewRoleActor, movieID, personID,
                new string[]
                {
                    "CharacterName"
                },
                new string[]
                {
                    "This role must have a character name."
                }
            };
            // CharacterName + Director
            yield return new object[]
            {
                characterName, crewRoleDirector, movieID, personID,
                new string[]
                {
                    "CharacterName"
                },
                new string[]
                {
                    "This role cannot have a character name."
                }
            };
            // Role = 100
            yield return new object[]
            {
                characterName, 100, movieID, personID,
                new string[]
                {
                    "Role"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // MovieID = 0
            yield return new object[]
            {
                characterName, crewRoleActor, 0, personID,
                new string[]
                {
                    "MovieID"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
            // PersonID = 0
            yield return new object[]
            {
                characterName, crewRoleActor, movieID, 0,
                new string[]
                {
                    "PersonID"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
            // Everything is wrong
            yield return new object[]
            {
                null, 100, 0, 0,
                new string[]
                {
                    "Role",
                    "MovieID",
                    "PersonID"
                },
                new string[]
                {
                    "Does not exist.",
                    "Must be above 0.",
                    "Must be above 0."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(string characterName, CrewRoles crewRole, int movieID, int personID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            await CreatePersonAndMovie();

            var newCrewMember = new AdminCrewMemberModel
            {
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/crewmember", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCrewMember.GetProperty("errors");
            var errors = errorProp.EnumerateObject();
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedErrorNames.Count(), errors.Count());
            Assert.All(expectedErrorNames, errorName => Assert.Contains(errorName, errors.Select(prop => prop.Name)));
            Assert.All(expectedErrorValues, errorValue => Assert.Contains(errorValue, errors.Select(prop => prop.Value[0].ToString())));
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors()
        {
            string characterName = "Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 2
            yield return new object[]
            {
                characterName, crewRoleActor, 2, personID,
                new string[]
                {
                    "MovieID"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // PersonID = 2
            yield return new object[]
            {
                characterName, crewRoleActor, movieID, 2,
                new string[]
                {
                    "PersonID"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // MovieID = 2 | PersonID = 2
            yield return new object[]
            {
                characterName, crewRoleActor, 2, 2,
                new string[]
                {
                    "MovieID",
                    "PersonID"
                },
                new string[]
                {
                    "Does not exist.",
                    "Does not exist."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors(string characterName, CrewRoles crewRole, int movieID, int personID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            await CreatePersonAndMovie();

            var newCrewMember = new AdminCrewMemberModel
            {
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/crewmember", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCrewMember.GetProperty("errors");
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
        public async Task Read_ValidRequest_ReturnsJsonResponseAndOk(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var data = await CreatePersonAndMovie();
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var crewMember = new Domain.CrewMember
            {
                CharacterName = "Character Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            };
            dbContext.CrewMembers.Add(crewMember);
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new CrewMemberModel
            {
                ID = crewMember.ID,
                CharacterName = crewMember.CharacterName,
                Role = crewMember.Role.ToString(),
                Movie = new MovieModel
                {
                    ID = movie.ID,
                    Title = movie.Title
                },
                Person = new PersonModel
                {
                    ID = person.ID,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                }
            };
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/crewmember/{id}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<CrewMemberModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Movie.Title, actualCrewMember.Movie.Title);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        public async Task Read_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/crewmember/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Fact]
        public async Task ReadAll_CrewMembersExist_ReturnsJsonResponseAndOk()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            await CreatePersonAndMovie();

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor,
                MovieID = 1,
                PersonID = 1
            });
            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer,
                MovieID = 1,
                PersonID = 1
            });
            await dbContext.SaveChangesAsync();

            int expectedCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/crewmember");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMembers = await JsonSerializer.DeserializeAsync<IEnumerable<CompanyModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualCrewMembers);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCount, actualCrewMembers.Count());
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoCrewMembersExist_ReturnsJsonResponseAndNoContent()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync("/api/crewmember");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "New Character Name", CrewRoles.Actor, 2, 2)]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string characterName, CrewRoles crewRole, int movieID, int personID)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var data = await CreatePersonAndMovie();
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var data2 = await CreatePersonAndMovie();
            var movie2 = data2[0] as Domain.Movie;
            var person2 = data2[1] as Domain.Person;

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };

            var expectedCrewMember = new CrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole.ToString(),
                Movie = new MovieModel
                {
                    ID = movie2.ID,
                    Title = movie2.Title 
                },
                Person = new PersonModel
                {
                    ID = person2.ID,
                    FirstName = person2.FirstName,
                    LastName = person2.LastName
                }
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{id}", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<CrewMemberModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Movie.Title, actualCrewMember.Movie.Title);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            Assert.Equal(expectedCrewMember.Person.FirstName, actualCrewMember.Person.FirstName);
            Assert.Equal(expectedCrewMember.Person.LastName, actualCrewMember.Person.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            int id = 1;
            string characterName = "New Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;
            int movieID = 2;
            int personID = 2;

            // CharacterName = null
            yield return new object[]
            {
                id, null, crewRoleActor, movieID, personID,
                new string[]
                {
                    "CharacterName"
                },
                new string[]
                {
                    "This role must have a character name."
                }
            };
            // CharacterName = empty
            yield return new object[]
            {
                id, "", crewRoleActor, movieID, personID,
                new string[]
                {
                    "CharacterName"
                },
                new string[]
                {
                    "This role must have a character name."
                }
            };
            // CharacterName + Director
            yield return new object[]
            {
                id, characterName, crewRoleDirector, movieID, personID,
                new string[]
                {
                    "CharacterName"
                },
                new string[]
                {
                    "This role cannot have a character name."
                }
            };
            // Role = 100
            yield return new object[]
            {
                id, characterName, 100, movieID, personID,
                new string[]
                {
                    "Role"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // MovieID = 0
            yield return new object[]
            {
                id, characterName, crewRoleActor, 0, personID,
                new string[]
                {
                    "MovieID"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
            // PersonID = 0
            yield return new object[]
            {
                id, characterName, crewRoleActor, movieID, 0,
                new string[]
                {
                    "PersonID"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
            // Everything is wrong
            yield return new object[]
            {
                0, null, 100, 0, 0,
                new string[]
                {
                    "Role",
                    "MovieID",
                    "PersonID"
                },
                new string[]
                {
                    "Does not exist.",
                    "Must be above 0.",
                    "Must be above 0."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(int id, string characterName, CrewRoles crewRole, int movieID, int personID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var data = await CreatePersonAndMovie();
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            var data2 = await CreatePersonAndMovie();
            var movie2 = data2[0] as Domain.Movie;
            var person2 = data2[1] as Domain.Person;

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{id}", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCrewMember.GetProperty("errors");
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
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = null,
                Role = CrewRoles.Writer,
                MovieID = 1,
                PersonID = 1
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{id}", newCrewMember);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors()
        {
            int id = 1;
            string characterName = "New Character Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 2 (does not exist)
            yield return new object[]
            {
                id, characterName, crewRoleActor, 2, personID,
                new string[]
                {
                    "MovieID"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // PersonID = 2 (does not exist)
            yield return new object[]
            {
                id, characterName, crewRoleActor, movieID, 2,
                new string[]
                {
                    "PersonID"
                },
                new string[]
                {
                    "Does not exist."
                }
            };
            // MovieID = 2 (does not exist)
            // PersonID = 2 (does not exist)
            yield return new object[]
            {
                id, characterName, crewRoleActor, 2, 2,
                new string[]
                {
                    "MovieID",
                    "PersonID"
                },
                new string[]
                {
                    "Does not exist.",
                    "Does not exist."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors(int id, string characterName, CrewRoles crewRole, int movieID, int personID, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var data = await CreatePersonAndMovie();
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{id}", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCrewMember.GetProperty("errors");
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

            dbContext.CrewMembers.Add(new Domain.CrewMember());
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/crewmember/{id}");
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
            var response = await client.DeleteAsync($"/api/crewmember/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }
    }
}
