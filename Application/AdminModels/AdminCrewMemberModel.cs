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
        public int ID { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }

        [EnumDataType(typeof(CrewRoles), ErrorMessage = "Does not exist.")]
        [JsonPropertyName("role")]
        public CrewRoles Role { get; set; }

        [JsonPropertyName("movieID")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be above 0.")]
        public int MovieID { get; set; }

        [JsonPropertyName("personID")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be above 0.")]
        public int PersonID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == CrewRoles.Actor && (CharacterName == null || CharacterName == ""))
            {
                yield return new ValidationResult("This role must have a character name.", new[] { nameof(CharacterName) });
            }

            if (Role != CrewRoles.Actor && CharacterName != null)
            {
                yield return new ValidationResult("This role cannot have a character name.", new[] { nameof(CharacterName) });
            }
        }
    }
}
