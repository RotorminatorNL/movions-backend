namespace Domain
{
    public class CompanyMovie
    {
        public int CompanyMovieID { get; set; }
        public Company Company { get; set; }
        public int CompanyID { get; set; }
        public Movie Movie { get; set; }
        public int MovieID { get; set; }
    }
}
