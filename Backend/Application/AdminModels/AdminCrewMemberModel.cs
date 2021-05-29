using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCrewMemberModel : IValidatableObject
    {
        [JsonPropertyName("id")]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }

        [EnumDataType(typeof(CrewRoles))]
        [JsonPropertyName("role")]
        public CrewRoles Role { get; set; }

        [JsonPropertyName("movieID")]
        [Range(1, int.MaxValue)]
        public int MovieID { get; set; }

        [JsonPropertyName("personID")]
        [Range(1, int.MaxValue)]
        public int PersonID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == CrewRoles.Actor && (CharacterName == null || CharacterName == ""))
            {
                yield return new ValidationResult("Actor should always have a character name.", new[] { nameof(CharacterName) });
            }

            if (Role != CrewRoles.Actor && CharacterName != null)
            {
                yield return new ValidationResult("Only an actor should have a character name.", new[] { nameof(CharacterName) });
            }
        }
    }
}
