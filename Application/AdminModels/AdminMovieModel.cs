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
        [Range(1, int.MaxValue)]
        public int ID { get; set; }

        [JsonPropertyName("description")]
        [Required]
        public string Description { get; set; }

        [JsonPropertyName("length")]
        [Range(1, int.MaxValue)]
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
