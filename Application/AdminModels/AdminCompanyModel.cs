using Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCompanyModel
    {
        [JsonPropertyName("id")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be above 0.")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Cannot be null or empty.")]
        public string Name { get; set; }

        [EnumDataType(typeof(CompanyTypes), ErrorMessage = "Does not exist.")]
        [JsonPropertyName("type")]
        public CompanyTypes Type { get; set; }
    }
}
