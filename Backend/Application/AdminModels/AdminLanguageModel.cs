using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminLanguageModel
    {
        [JsonPropertyName("id")]
        [Required]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
    }
}
