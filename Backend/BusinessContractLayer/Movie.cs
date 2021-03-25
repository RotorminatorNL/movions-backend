using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessContractLayer
{
    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Language> Languages { get; set; }
        public List<Company> Companies { get; set; }
        public DateTime Length { get; set; }
        public DateTime ReleaseDate { get; set; }

        public IEnumerable<CrewRole> Crew { get; set; }
    }
}
