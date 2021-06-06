using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminPersonModel
    {
        [JsonPropertyName("id")]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }

        [JsonPropertyName("birthDate")]
        [Required]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("birthPlace")]
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
    }
}
