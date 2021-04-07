using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Movie
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime Length { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Title { get; set; }
        public IEnumerable<MovieCompany> Companies { get; set; }
        public IEnumerable<CrewRole> Crew { get; set; }
        public IEnumerable<MovieGenre> Genres { get; set; }
        public Language Language { get; set; }
    }
}
