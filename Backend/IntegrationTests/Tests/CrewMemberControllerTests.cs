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

        public static IEnumerable<object[]> CreateInvalidRequestData()
        {
            string characterName = "Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;
            int movieID = 1;
            int personID = 1;

            // object = null
            yield return new object[] { "null", 0, movieID, personID, new string[] { "" } };
            // object = wrong
            yield return new object[] { "wrongModel", 0, movieID, personID, new string[] { "$" } };
            // Actor's character name cannot be null
            yield return new object[] { null, crewRoleActor, movieID, personID, new string[] { "CharacterName" } };
            // Actor's character name cannot be empty
            yield return new object[] { "", crewRoleActor, movieID, personID, new string[] { "CharacterName" } };
            // Director's character name must be null
            yield return new object[] { characterName, crewRoleDirector, movieID, personID, new string[] { "CharacterName" } };
            // Crew role must exist
            yield return new object[] { characterName, 100, movieID, personID, new string[] { "Role" } };
            // Movie cannot be 0
            yield return new object[] { characterName, crewRoleActor, 0, personID, new string[] { "MovieID" } };
            // Person cannot be 0
            yield return new object[] { characterName, crewRoleActor, movieID, 0, new string[] { "PersonID" } };
            // Everything is wrong
            yield return new object[] { null, 100, 0, 0, new string[] { "Role", "MovieID", "PersonID" } };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidRequestData))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequest(string characterName, CrewRoles crewRole, int movieID, int personID, IEnumerable<string> expectedErrors)
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
            newCrewMember = newCrewMember.CharacterName == "null" ? null : newCrewMember;

            var wrongModel = new object[] { 0, characterName, crewRole, movieID, personID };
            #endregion

            #region Act
            var response = characterName == "wrongModel"
                ? await client.PostAsJsonAsync("/api/crewmember", wrongModel)
                : await client.PostAsJsonAsync("/api/crewmember", newCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualCrewMember.GetProperty("errors");
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

            int expectedCompanyCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/crewmember");
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

        public static IEnumerable<object[]> UpdateInvalidRequestData()
        {
            int id = 1;
            string characterName = "Some other name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleWriter = CrewRoles.Writer;
            int movieID = 1;
            int personID = 1;

            // object = null
            yield return new object[] { 0, "null", 0, movieID, personID, new string[] { "" } };
            // object = wrong
            yield return new object[] { 0, "wrongModel", 0, movieID, personID, new string[] { "$" } };
            // Actor's character name cannot be null
            yield return new object[] { id, null, crewRoleActor, movieID, personID, new string[] { "CharacterName" } };
            // Actor's character name cannot be empty
            yield return new object[] { id, "", crewRoleActor, movieID, personID, new string[] { "CharacterName" } };
            // Writer's character name must be null
            yield return new object[] { id, characterName, crewRoleWriter, movieID, personID, new string[] { "CharacterName" } };
            // Crew role must exist
            yield return new object[] { id, characterName, 100, movieID, personID, new string[] { "Role" } };
            // Movie cannot be 0
            yield return new object[] { id, characterName, crewRoleActor, 0, personID, new string[] { "MovieID" } };
            // Person cannot be 0
            yield return new object[] { id, characterName, crewRoleActor, movieID, 0, new string[] { "PersonID" } };
            // Everything is wrong
            yield return new object[] { id, null, 100, 0, 0, new string[] { "Role", "MovieID", "PersonID" } };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidRequestData))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id, string characterName, CrewRoles crewRole, int movieID, int personID, IEnumerable<string> expectedErrors)
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
