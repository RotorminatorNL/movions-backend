using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCompanyMovieModel
    {
        [JsonPropertyName("companyID")]
        [Required]
        public int CompanyID { get; set; }

        [JsonPropertyName("movieID")]
        [Required]
        public int MovieID { get; set; }
    }
}
