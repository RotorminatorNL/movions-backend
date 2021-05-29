using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminMovieModel
    {
        [JsonPropertyName("id")]
        [Required]
        public int ID { get; set; }

        [JsonPropertyName("description")]
        [Required]
        public string Description { get; set; }

        [JsonPropertyName("length")]
        [Required]
        public int Length { get; set; }

        [JsonPropertyName("releaseDate")]
        [Required]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("title")]
        [Required]
        public string Title { get; set; }

        [JsonPropertyName("language")]
        [Required]
        public AdminLanguageModel Language { get; set; }
    }
}
