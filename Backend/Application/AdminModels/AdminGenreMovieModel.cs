using System.Text.Json.Serialization;

namespace Application.AdminModels
{
    public class AdminGenreMovieModel
    {
        [JsonPropertyName("genreID")]
        public int GenreID { get; set; }

        [JsonPropertyName("movieID")]
        public int MovieID { get; set; }
    }
}
