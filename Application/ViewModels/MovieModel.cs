using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.ViewModels
{
    public class MovieModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("companies")]
        public IEnumerable<CompanyModel> Companies { get; set; }

        [JsonPropertyName("crew")]
        public IEnumerable<CrewMemberModel> Crew { get; set; }

        [JsonPropertyName("genres")]
        public IEnumerable<GenreModel> Genres { get; set; }

        [JsonPropertyName("language")]
        public LanguageModel Language { get; set; }
    }
}
