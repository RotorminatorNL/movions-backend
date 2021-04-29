using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class Validation
    {
        public bool MovieCheck(AdminMovieModel adminMovieModel)
        {
            if(adminMovieModel.Description == null || adminMovieModel.Length == 0 || adminMovieModel.ReleaseDate == null || adminMovieModel.Title == null)
            {
                return true;
            }

            return false;
        }
    }
}
