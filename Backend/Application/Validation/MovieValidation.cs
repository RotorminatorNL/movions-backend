using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application
{
    public class MovieValidation
    { 
        public bool MovieCreateCheck(AdminMovieModel adminMovieModel)
        {
            bool isCompaniesOk = adminMovieModel.Companies == null || !adminMovieModel.Companies.ToList().Any();
            bool isDescriptionOk = adminMovieModel.Description == null || adminMovieModel.Description == "";
            bool isGenresOk = adminMovieModel.Genres == null || !adminMovieModel.Genres.ToList().Any();
            bool isLanguageOk = adminMovieModel.Language == null || adminMovieModel.Language.ID == 0;
            bool isLengthOk = adminMovieModel.Length == 0;
            bool isReleaseDateOk = adminMovieModel.ReleaseDate == DateTime.Parse("1-1-0001 00:00:00");
            bool isTitleOk = adminMovieModel.Title == null || adminMovieModel.Title == "";

            if (isCompaniesOk || isDescriptionOk || isGenresOk || isLanguageOk || isLengthOk || 
                isReleaseDateOk || isTitleOk)
            {
                return true;
            }

            return false;
        }
    }
}
