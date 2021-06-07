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
    public class PersonControllerTests : IntegrationTestSetup
    {
        public PersonControllerTests(ApiFactory<Startup> factory)
            : base(factory)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");
        }

        [Theory]
        [InlineData("04-10-2010", "Birth Place", "Description", "First Name", "Last Name")]
        public async Task Create_ValidRequest_ReturnsJsonResponseAndCreated(string birthDate, string birthPlace, string description, string firstName, string lastName)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newPerson = new AdminPersonModel
            {
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var expectedPerson = new PersonModel
            {
                ID = 1,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/person", newPerson);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualPerson = await JsonSerializer.DeserializeAsync<PersonModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedPerson.ID, actualPerson.ID);
            Assert.Equal(expectedPerson.BirthDate, actualPerson.BirthDate);
            Assert.Equal(expectedPerson.BirthPlace, actualPerson.BirthPlace);
            Assert.Equal(expectedPerson.Description, actualPerson.Description);
            Assert.Equal(expectedPerson.FirstName, actualPerson.FirstName);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            var birthDate = "04-10-2010";
            var birthPlace = "Birth Place";
            var descripton = "Description";
            var firstName = "First Name";
            var lastName = "Last Name";

            // birthDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[] 
            { 
                "1-1-0001 00:00:00", birthPlace, descripton, firstName, lastName,
                new string[]
                {
                    "BirthDate"
                },
                new string[]
                { 
                    "Must be later than 1-1-0001 00:00:00."
                }
            };
            // birthPlace = null
            yield return new object[] 
            { 
                birthDate, null, descripton, firstName, lastName,
                new string[]
                {
                    "BirthPlace"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // birthPlace = empty
            yield return new object[]
            { 
                birthDate, "", descripton, firstName, lastName,
                new string[]
                {
                    "BirthPlace"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // descripton = null
            yield return new object[] 
            { 
                birthDate, birthPlace, null, firstName, lastName,
                new string[]
                {
                    "Description"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // descripton = empty
            yield return new object[] 
            {
                birthDate, birthPlace, "", firstName, lastName,
                new string[]
                {
                    "Description"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // firstName = null
            yield return new object[]
            { 
                birthDate, birthPlace, descripton, null, lastName,
                new string[]
                {
                    "FirstName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // firstName = empty
            yield return new object[] 
            {
                birthDate, birthPlace, descripton, "", lastName,
                new string[]
                {
                    "FirstName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // lastName = null
            yield return new object[] 
            { 
                birthDate, birthPlace, descripton, firstName, null,
                new string[]
                {
                    "LastName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // lastName = empty
            yield return new object[] 
            { 
                birthDate, birthPlace, descripton, firstName, "",
                new string[]
                {
                    "LastName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Create_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(string birthDate, string birthPlace, string description, string firstName, string lastName, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorValues)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();

            var newPerson = new AdminPersonModel
            {
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };
            #endregion

            #region Act
            var response = await client.PostAsJsonAsync("/api/person", newPerson);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualPerson = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualPerson.GetProperty("errors");
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

            var person = new Domain.Person
            {
                BirthDate = "04-10-2010",
                BirthPlace = "Birth Place",
                Description = "Description",
                FirstName = "First Name",
                LastName = "Last Name"
            };
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            var expectedPerson = new PersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName
            };
            #endregion

            #region Act
            var response = await client.GetAsync($"/api/person/{id}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualPerson = await JsonSerializer.DeserializeAsync<PersonModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedPerson.ID, actualPerson.ID);
            Assert.Equal(expectedPerson.BirthDate, actualPerson.BirthDate);
            Assert.Equal(expectedPerson.BirthPlace, actualPerson.BirthPlace);
            Assert.Equal(expectedPerson.Description, actualPerson.Description);
            Assert.Equal(expectedPerson.FirstName, actualPerson.FirstName);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
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
            var response = await client.GetAsync($"/api/person/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }

        [Fact]
        public async Task ReadAll_PersonsExist_ReturnsJsonResponseAndOk()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            dbContext.Persons.Add(new Domain.Person
            {
                BirthDate = "04-10-2010"
            });
            dbContext.Persons.Add(new Domain.Person
            {
                BirthDate = "04-10-2010"
            });
            await dbContext.SaveChangesAsync();

            int expectedCount = 2;
            #endregion

            #region Act
            var response = await client.GetAsync("/api/person");
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualPersons = await JsonSerializer.DeserializeAsync<IEnumerable<PersonModel>>(responseBody);
            #endregion

            #region Assert
            Assert.NotNull(actualPersons);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCount, actualPersons.Count());
            #endregion
        }

        [Fact]
        public async Task ReadAll_NoPersonsExist_ReturnsJsonResponseAndNoContent()
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            #endregion

            #region Act
            var response = await client.GetAsync("/api/person");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(1, "10-10-2010", "New Birth Place", "New Description", "New First Name", "New Last Name")]
        public async Task Update_ValidRequest_ReturnsJsonResponseAndOk(int id, string birthDate, string birthPlace, string description, string firstName, string lastName)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var person = new Domain.Person
            {
                BirthDate = "04-10-2010",
                BirthPlace = "Birth Place",
                Description = "Description",
                FirstName = "First Name",
                LastName = "Last Name"
            };
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            var newPerson = new AdminPersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };

            var expectedPerson = new PersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/person/{id}", newPerson);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualPerson = await JsonSerializer.DeserializeAsync<PersonModel>(responseBody);
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedPerson.ID, actualPerson.ID);
            Assert.Equal(expectedPerson.BirthDate, actualPerson.BirthDate);
            Assert.Equal(expectedPerson.BirthPlace, actualPerson.BirthPlace);
            Assert.Equal(expectedPerson.Description, actualPerson.Description);
            Assert.Equal(expectedPerson.FirstName, actualPerson.FirstName);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
            #endregion
        }

        public static IEnumerable<object[]> Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors()
        {
            int id = 1;
            var birthDate = "10-10-2010";
            var birthPlace = "New Birth Place";
            var descripton = "New Description";
            var firstName = "New First Name";
            var lastName = "New Last Name";

            // birthDate = null (i.e. "1-1-0001 00:00:00")
            yield return new object[]
            {
                id, "1-1-0001 00:00:00", birthPlace, descripton, firstName, lastName,
                new string[]
                {
                    "BirthDate"
                },
                new string[]
                {
                    "Must be later than 1-1-0001 00:00:00."
                }
            };
            // birthPlace = null
            yield return new object[]
            {
                id, birthDate, null, descripton, firstName, lastName,
                new string[]
                {
                    "BirthPlace"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // birthPlace = empty
            yield return new object[]
            {
                id, birthDate, "", descripton, firstName, lastName,
                new string[]
                {
                    "BirthPlace"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // descripton = null
            yield return new object[]
            {
                id, birthDate, birthPlace, null, firstName, lastName,
                new string[]
                {
                    "Description"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // descripton = empty
            yield return new object[]
            {
                id, birthDate, birthPlace, "", firstName, lastName,
                new string[]
                {
                    "Description"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // firstName = null
            yield return new object[]
            {
                id, birthDate, birthPlace, descripton, null, lastName,
                new string[]
                {
                    "FirstName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // firstName = empty
            yield return new object[]
            {
                id, birthDate, birthPlace, descripton, "", lastName,
                new string[]
                {
                    "FirstName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // lastName = null
            yield return new object[]
            {
                id, birthDate, birthPlace, descripton, firstName, null,
                new string[]
                {
                    "LastName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
            // lastName = empty
            yield return new object[]
            {
                id, birthDate, birthPlace, descripton, firstName, "",
                new string[]
                {
                    "LastName"
                },
                new string[]
                {
                    "Cannot be null or empty."
                }
            };
        }

        [Theory]
        [MemberData(nameof(Data_Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors))]
        public async Task Update_InvalidRequest_ReturnsJsonResponseAndBadRequestWithErrors(int id, string birthDate, string birthPlace, string description, string firstName, string lastName, IEnumerable<string> expectedErrorNames, IEnumerable<string> expectedErrorMessages)
        {
            #region Arrange 
            await DeleteDbContent();
            var client = GetHttpClient();
            var dbContext = GetDbContext();

            var person = new Domain.Person
            {
                BirthDate = "04-10-2010",
                BirthPlace = "Birth Place",
                Description = "Description",
                FirstName = "First Name",
                LastName = "Last Name"
            };
            dbContext.Persons.Add(person);
            await dbContext.SaveChangesAsync();

            var newPerson = new AdminPersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse(birthDate),
                BirthPlace = birthPlace,
                Description = description,
                FirstName = firstName,
                LastName = lastName
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/person/{id}", newPerson);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var actualPerson = await JsonSerializer.DeserializeAsync<JsonElement>(responseBody);

            var errorProp = actualPerson.GetProperty("errors");
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

            var newPerson = new AdminPersonModel
            {
                ID = id,
                BirthDate = DateTime.Parse("04-10-2010"),
                BirthPlace = "Birth Place",
                Description = "Description",
                FirstName = "First Name",
                LastName = "Last Name"
            };
            #endregion

            #region Act
            var response = await client.PutAsJsonAsync($"/api/person/{id}", newPerson);
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

            dbContext.Persons.Add(new Domain.Person());
            await dbContext.SaveChangesAsync();
            #endregion

            #region Act
            var response = await client.DeleteAsync($"/api/person/{id}");
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
            var response = await client.DeleteAsync($"/api/person/{id}");
            #endregion

            #region Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            #endregion
        }
    }
}
