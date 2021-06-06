using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminCompanyMovieModel
    {
        [JsonPropertyName("CompanyID")]
        public int CompanyID { get; set; }

        [JsonPropertyName("MOVIEID")]
        public int MovieID { get; set; }
    }
}
