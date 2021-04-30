using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class Validation
    {
        public bool MovieCheck(AdminMovieModel adminMovieModel)
        {
            if(adminMovieModel.Description == null || 
                adminMovieModel.Description == "" ||
                adminMovieModel.Language == null ||
                adminMovieModel.Language.ID == 0 ||
                adminMovieModel.Length == 0 ||
                adminMovieModel.ReleaseDate == null ||
                adminMovieModel.ReleaseDate == DateTime.Parse("4-10-2010 12:12:12") ||
                adminMovieModel.Title == null || 
                adminMovieModel.Title == "")
            {
                return true;
            }

            return false;
        }
    }
}
