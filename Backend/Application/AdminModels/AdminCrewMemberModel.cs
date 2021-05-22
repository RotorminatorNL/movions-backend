using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application
{
    public class AdminCrewMemberModel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }

        [JsonPropertyName("role")]
        public CrewRoles Role { get; set; }

        [JsonPropertyName("movieID")]
        public int MovieID { get; set; }

        [JsonPropertyName("movie")]
        public AdminMovieModel Movie { get; set; }

        [JsonPropertyName("personID")]
        public int PersonID { get; set; }

        [JsonPropertyName("person")]
        public AdminPersonModel Person { get; set; }
    }
}
