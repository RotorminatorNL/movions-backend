using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminGenreModel
    {
        [JsonPropertyName("id")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be above 0.")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Cannot be null or empty.")]
        public string Name { get; set; }
    }
}
