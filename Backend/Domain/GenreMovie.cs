namespace Domain
{
    public class GenreMovie
    {
        public int GenreMovieID { get; set; }
        public Genre Genre { get; set; }
        public int GenreID { get; set; }
        public Movie Movie { get; set; }
        public int MovieID { get; set; }
    }
}
