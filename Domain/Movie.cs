using System.Collections.Generic;

namespace Domain
{
    public class Movie
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public IEnumerable<CompanyMovie> Companies { get; set; }
        public IEnumerable<CrewMember> Crew { get; set; }
        public IEnumerable<GenreMovie> Genres { get; set; }
        public Language Language { get; set; }
        public int LanguageID { get; set; }
    }
}
