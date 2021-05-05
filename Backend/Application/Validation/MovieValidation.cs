using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application
{
    public class MovieValidation
    { 
        public bool CreateMovie(AdminMovieModel adminMovieModel)
        {
            bool isCompanyOk = !(adminMovieModel.Companies == null || !adminMovieModel.Companies.ToList().Any());
            bool isCrewOk = !(adminMovieModel.Crew == null || !adminMovieModel.Crew.ToList().Any());
            bool isDescriptionOk = !(adminMovieModel.Description == null || adminMovieModel.Description == "");
            bool isGenreOk = !(adminMovieModel.Genres == null || !adminMovieModel.Genres.ToList().Any());
            bool isLanguageOk = !(adminMovieModel.Language == null || adminMovieModel.Language.ID == 0);
            bool isLengthOk = adminMovieModel.Length != 0;
            bool isReleaseDateOk = adminMovieModel.ReleaseDate != DateTime.Parse("1-1-0001 00:00:00");
            bool isTitleOk = !(adminMovieModel.Title == null || adminMovieModel.Title == "");

            return isCompanyOk && isCrewOk && isDescriptionOk && isGenreOk && 
                isLanguageOk && isLengthOk && isReleaseDateOk && isTitleOk;
        }
    }
}
