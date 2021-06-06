using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.ViewModels
{
    public class CrewMemberModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("movie")]
        public MovieModel Movie { get; set; }

        [JsonPropertyName("person")]
        public PersonModel Person { get; set; }
    }
}
