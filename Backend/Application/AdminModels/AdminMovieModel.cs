using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application
{
    public class AdminMovieModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("companies")]
        public IEnumerable<AdminCompanyModel> Companies { get; set; }

        [JsonPropertyName("crew")]
        public IEnumerable<AdminCrewMemberModel> Crew { get; set; }

        [JsonPropertyName("genres")]
        public IEnumerable<AdminGenreModel> Genres { get; set; }

        [JsonPropertyName("language")]
        public AdminLanguageModel Language { get; set; }
    }
}
