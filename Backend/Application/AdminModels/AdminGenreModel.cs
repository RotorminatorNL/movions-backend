using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application
{
    public class AdminGenreModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("movies")]
        public IEnumerable<AdminMovieModel> Movies { get; set; }
    }
}
