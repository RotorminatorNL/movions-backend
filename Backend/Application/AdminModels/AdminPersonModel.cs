using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application
{
    public class AdminPersonModel
    {
        [JsonPropertyName("id")]
        [Required]
        public int ID { get; set; }

        [JsonPropertyName("birthDate")]
        [Required]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("birthDate")]
        [Required]
        public string BirthPlace { get; set; }

        [JsonPropertyName("description")]
        [Required]
        public string Description { get; set; }

        [JsonPropertyName("firstName")]
        [Required]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [Required]
        public string LastName { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<AdminCrewMemberModel> Roles { get; set; }
    }
}
