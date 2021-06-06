using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminMovieModel : IValidatableObject
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("description")]
        [Required(ErrorMessage = "Cannot be null or empty.")]
        public string Description { get; set; }

        [JsonPropertyName("languageId")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be above 0.")]
        public int LanguageID { get; set; }

        [JsonPropertyName("length")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be above 0.")]
        public int Length { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Cannot be null or empty.")]
        public string Title { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReleaseDate == new DateTime())
            {
                yield return new ValidationResult("Must be later than 1-1-0001 00:00:00.", new[] { nameof(ReleaseDate) });
            }
        }
    }
}
