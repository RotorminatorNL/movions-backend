using Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCompanyModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Cannot be null or empty.")]
        public string Name { get; set; }

        [EnumDataType(typeof(CompanyTypes), ErrorMessage = "Does not exist.")]
        [JsonPropertyName("type")]
        public CompanyTypes Type { get; set; }
    }
}
