using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCrewMemberModel
    {
        [JsonPropertyName("id")]
        [Required]
        public int ID { get; set; }

        [JsonPropertyName("characterName")]
        [Required]
        public string CharacterName { get; set; }

        [EnumDataType(typeof(CrewRoles))]
        [JsonPropertyName("role")]
        [Required]
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
