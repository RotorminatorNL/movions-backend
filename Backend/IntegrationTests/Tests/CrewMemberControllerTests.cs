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
        public CrewMemberControllerTests(ApiFactory<Startup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("Character Name", CrewRoles.Actor)]
        [InlineData(null, CrewRoles.Director)]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string name, CrewRoles crewRole)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = GetHttpClient();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = 1,
                CharacterName = name,
                Role = crewRole
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/crewmember", expectedCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<AdminCrewMemberModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedCrewMember.ID, actualCrewMember.ID);
            Assert.Equal(expectedCrewMember.CharacterName, actualCrewMember.CharacterName);
            Assert.Equal(expectedCrewMember.Role, actualCrewMember.Role);
            #endregion
        }

        public static IEnumerable<object[]> CreateInvalidRequestData()
        {
            string characterName = "Name";
            CrewRoles crewRoleActor = CrewRoles.Actor;
            CrewRoles crewRoleDirector = CrewRoles.Director;

            // object = null
            yield return new object[] { "null", 0, new string[] { "" } };
            // object = wrong model
            yield return new object[] { "wrongModel", 0, new string[] { "$" } };
            // Actor should have a character name
            yield return new object[] { null, crewRoleActor, new string[] { "CharacterName" } };
            // Actor should have a character name
            yield return new object[] { "", crewRoleActor, new string[] { "CharacterName" } };
            // Director should not have a character name
            yield return new object[] { characterName, crewRoleDirector, new string[] { "CharacterName" } };
            // crewRole = 100 (does not exists)
            yield return new object[] { characterName, 100, new string[] { "Role" } };
        }

        [Theory]
        [MemberData(nameof(CreateInvalidRequestData))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequest(string characterName, CrewRoles crewRole, IEnumerable<string> expectedErrors)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = GetHttpClient();

            var invalidCompanyData = new AdminCrewMemberModel
            {
                ID = 1,
                CharacterName = characterName,
                Role = crewRole
            };
            invalidCompanyData = invalidCompanyData.CharacterName == "null" ? null : invalidCompanyData;

            var wrongModel = new object[] { 0, characterName, crewRole };
            #endregion

            #region Act
            var response = characterName == "wrongModel"
                ? await client.PostAsJsonAsync("/api/crewmember", wrongModel)
                : await client.PostAsJsonAsync("/api/crewmember", invalidCompanyData);
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor
            });
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new CrewMemberModel
            {
                ID = 1,
                CharacterName = "Name",
                Role = CrewRoles.Actor.ToString()
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor
            });
            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer
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
        [InlineData(1, "Some other name", CrewRoles.Actor)]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string characterName, CrewRoles crewRole)
        {
            #region Arrange 
            await DeleteDbContent();

            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                Role = CrewRoles.Writer
            });
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{expectedCrewMember.ID}", expectedCrewMember);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualCrewMember = await JsonSerializer.DeserializeAsync<AdminCrewMemberModel>(responseBody);
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

            // object = null
            yield return new object[] { 0, "null", 0, new string[] { "" } };
            // object = wrong
            yield return new object[] { 0, "wrongModel", 0, new string[] { "$" } };
            // Actor should have a character name
            yield return new object[] { id, null, crewRoleActor, new string[] { "CharacterName" } };
            // Actor should always have a character name
            yield return new object[] { id, "", crewRoleActor, new string[] { "CharacterName" } };
            // Only an actor should have a character name
            yield return new object[] { id, characterName, crewRoleWriter, new string[] { "CharacterName" } };
            // companyType = 100 (does not exist)
            yield return new object[] { id, characterName, 100, new string[] { "Role" } };
        }

        [Theory]
        [MemberData(nameof(UpdateInvalidRequestData))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequest(int id, string characterName, CrewRoles crewRole, IEnumerable<string> expectedErrors)
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

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                CharacterName = characterName,
                Role = crewRole
            };
            expectedCrewMember = expectedCrewMember.CharacterName == "null" ? null : expectedCrewMember;

            var wrongModel = new object[] { id, characterName, crewRole };
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

            dbContext.CrewMembers.Add(new Domain.CrewMember
            {
                CharacterName = "Name",
                Role = CrewRoles.Actor
            });
            await dbContext.SaveChangesAsync();

            var expectedCrewMember = new AdminCrewMemberModel
            {
                ID = id,
                Role = CrewRoles.Writer
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/crewmember/{id}", expectedCrewMember);
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
