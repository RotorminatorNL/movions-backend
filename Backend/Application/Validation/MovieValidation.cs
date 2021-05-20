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
            bool isDescriptionOk = !(adminMovieModel.Description == null || adminMovieModel.Description == "");
            bool isLanguageOk = !(adminMovieModel.Language == null || adminMovieModel.Language.ID == 0);
            bool isLengthOk = adminMovieModel.Length != 0;
            bool isReleaseDateOk = adminMovieModel.ReleaseDate != DateTime.Parse("1-1-0001 00:00:00");
            bool isTitleOk = !(adminMovieModel.Title == null || adminMovieModel.Title == "");

            return isDescriptionOk && isLanguageOk && isLengthOk && isReleaseDateOk && isTitleOk;
        }
    }
}
