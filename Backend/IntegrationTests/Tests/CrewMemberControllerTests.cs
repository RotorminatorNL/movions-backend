using API;
using Application.AdminModels;
using Application.ViewModels;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
                Title = "Epic Movie"
            };

            var person = new Domain.Person
            {
                FirstName = "Don",
                LastName = "Diablo"
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
                ID = 1,
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
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            string characterName = "Name";
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

            var data = await CreatePersonAndMovie();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = 1,
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
            string characterName = "Name";
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

            var data = await CreatePersonAndMovie();

            var newCrewMember = new AdminCrewMemberModel
            {
                ID = 1,
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new CrewMemberModel
            {
                ID = 1,
                CharacterName = "Name",
                Role = CrewRoles.Actor.ToString(),
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
            Assert.Equal(expectedCrewMember.Movie.ID, actualCrewMember.Movie.ID);
            Assert.Equal(expectedCrewMember.Person.ID, actualCrewMember.Person.ID);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task Read_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id)
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
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/crewmember/{id}");
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

            var data = await CreatePersonAndMovie();
            var movie = data[0] as Domain.Movie;
            var person = data[1] as Domain.Person;

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            int expectedCrewMemberCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/crewmember");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMembers = await JsonSerializer.DeserializeAsync<IEnumerable<CompanyModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualCrewMembers);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCrewMemberCount, actualCrewMembers.Count());
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
            var response = await client.GetAsync("/api/crewmember");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "Some other name", CrewRoles.Actor, 1, 1)]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string characterName, CrewRoles crewRole, int movieID, int personID)
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

            var expectedCrewMember = new CrewMemberModel
            {
                ID = id,
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
            var response = await client.PutAsJsonAsync($"/api/crewmember/{newCrewMember.ID}", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<CrewMemberModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            int id = 1;
            string characterName = "Some other name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;
            int movieID = 1;
            int personID = 1;

            // ID = 0
            yield return new object[]
            {
                0, characterName, crewRoleActor, movieID, personID,
                new string[]
                {
                    "ID"
                },
                new string[]
                {
                    "Must be above 0."
                }
            };
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
                    "ID",
                    "Role",
                    "MovieID",
                    "PersonID"
                },
                new string[]
                {
                    "Must be above 0.",
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{id}", expectedCrewMember);
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
        [InlineData(2)]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
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
                CharacterName = "Diego Lopez",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var newCrewMemeber = new AdminCrewMemberModel
            {
                ID = id,
                Role = CrewRoles.Writer,
                MovieID = movie.ID,
                PersonID = person.ID
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{newCrewMemeber.ID}", newCrewMemeber);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndNotFoundWithErrors()
        {
            int id = 1;
            string characterName = "Some other name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            int movieID = 1;
            int personID = 1;

            // MovieID = 2
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
            // PersonID = 2
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
            // MovieID = 2 | PersonID = 2
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
                CharacterName = "Name",
                Role = CrewRoles.Actor,
                MovieID = movie.ID,
                PersonID = person.ID
            });
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole,
                MovieID = movieID,
                PersonID = personID
            };
            expectedCrewMember = expectedCrewMember.CharacterName == "null" ? null : expectedCrewMember;

            var wrongModel = Array.Empty<object>();
            #endregion

            #region Act
            var response = characterName == "wrongModel"
                ? await client.PutAsJsonAsync($"/api/crewmember/{id}", wrongModel)
                : await client.PutAsJsonAsync($"/api/crewmember/{id}", expectedCrewMember);
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor
            });
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
        [InlineData(0)]
        [InlineData(2)]
        public async Task Delete_InvalidRequest_ReturnsJsonResponseAndNotFound(int id)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor
            });
            await dbContext.SaveChangesAsync();
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
