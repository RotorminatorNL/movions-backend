using Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCompanyModel
    {
        [JsonPropertyName("id")]
        [Required]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [EnumDataType(typeof(CompanyTypes))]
        [JsonPropertyName("type")]
        [Required]
        public CompanyTypes Type { get; set; }

        [JsonPropertyName("movies")]
        public IEnumerable<AdminMovieModel> Movies { get; set; }
    }
}
