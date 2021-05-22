using Domain.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application
{
    public class AdminCompanyModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public CompanyTypes Type { get; set; }

        [JsonPropertyName("movies")]
        public IEnumerable<AdminMovieModel> Movies { get; set; }
    }
}
