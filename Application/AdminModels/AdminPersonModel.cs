using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminPersonModel : IValidatableObject
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("birthPlace")]
        public string BirthPlace { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BirthDate == new DateTime())
            {
                yield return new ValidationResult("Must be later than 1-1-0001 00:00:00.", new[] { nameof(BirthDate) });
            }

            if (BirthPlace == null || BirthPlace == "")
            {
                yield return new ValidationResult("Cannot be null or empty.", new[] { nameof(BirthPlace) });
            }

            if (Description == null || Description == "")
            {
                yield return new ValidationResult("Cannot be null or empty.", new[] { nameof(Description) });
            }

            if (FirstName == null || FirstName == "")
            {
                yield return new ValidationResult("Cannot be null or empty.", new[] { nameof(FirstName) });
            }

            if (LastName == null || LastName == "")
            {
                yield return new ValidationResult("Cannot be null or empty.", new[] { nameof(LastName) });
            }
        }
    }
}
