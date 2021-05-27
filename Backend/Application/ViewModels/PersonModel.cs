using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.ViewModels
{
    public class PersonModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("birthDate")]
        public string BirthPlace { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<CrewMemberModel> Roles { get; set; }
    }
}
