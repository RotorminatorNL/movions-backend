using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class AdminMovieModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Title { get; set; }
        public IEnumerable<AdminCompanyModel> Companies { get; set; }
        public IEnumerable<AdminCrewRoleModel> Crew { get; set; }
        public IEnumerable<AdminGenreModel> Genres { get; set; }
        public AdminLanguageModel Language { get; set; }
    }
}
