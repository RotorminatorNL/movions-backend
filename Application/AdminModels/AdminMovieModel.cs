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
        public string Description { get; set; }

        [JsonPropertyName("languageId")]
        public int LanguageID { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Description == null || Description == "")
            {
                yield return new ValidationResult("Cannot be null or empty.", new[] { nameof(Description) });
            }

            if (LanguageID <= 0 )
            {
                yield return new ValidationResult("Must be above 0.", new[] { nameof(LanguageID) });
            }

            if (Length <= 0)
            {
                yield return new ValidationResult("Must be above 0.", new[] { nameof(Length) });
            }

            if (ReleaseDate == new DateTime())
            {
                yield return new ValidationResult("Must be later than 1-1-0001 00:00:00.", new[] { nameof(ReleaseDate) });
            }

            if (Title == null || Title == "")
            {
                yield return new ValidationResult("Cannot be null or empty.", new[] { nameof(Title) });
            }
        }
    }
}
