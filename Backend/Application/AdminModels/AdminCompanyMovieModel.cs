using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCompanyMovieModel
    {
        [JsonPropertyName("companyID")]
        [Range(1, int.MaxValue)]
        public int CompanyID { get; set; }

        [JsonPropertyName("movieID")]
        [Range(1, int.MaxValue)]
        public int MovieID { get; set; }
    }
}
