using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application
{
    public class MovieValidation
    {
        public bool IsInputValid(AdminMovieModel adminMovieModel)
        {
            bool isDescriptionOk = !(adminMovieModel.Description == null || adminMovieModel.Description == "");
            bool isLanguageOk = !(adminMovieModel.Language == null || adminMovieModel.Language.ID == 0);
            bool isLengthOk = adminMovieModel.Length != 0;
            bool isReleaseDateOk = adminMovieModel.ReleaseDate != DateTime.Parse("1-1-0001 00:00:00");
            bool isTitleOk = !(adminMovieModel.Title == null || adminMovieModel.Title == "");

            return isDescriptionOk && isLanguageOk && isLengthOk && isReleaseDateOk && isTitleOk;
        }

        public bool IsInputDataDifferent(Domain.Movie movie, AdminMovieModel adminMovieModel)
        {
            bool isDescriptionDifferent = movie.Description != adminMovieModel.Description;
            bool isLanguageDifferent = movie.LanguageID != adminMovieModel.Language.ID;
            bool isLengthDifferent = movie.Length != adminMovieModel.Length;
            bool isReleaseDateDifferent = movie.ReleaseDate != adminMovieModel.ReleaseDate.ToString();
            bool isTitleDifferent = movie.Title != adminMovieModel.Title;

            Console.WriteLine("--------------------");
            Console.WriteLine("Desc: " + isDescriptionDifferent);
            Console.WriteLine("Lang: " + isLanguageDifferent);
            Console.WriteLine("Length: " + isLengthDifferent);
            Console.WriteLine("Release: " + isReleaseDateDifferent);
            Console.WriteLine("Release - Domain.Movie: " + movie.ReleaseDate);
            Console.WriteLine("Release - adminMovieModel.ReleaseDate: " + adminMovieModel.ReleaseDate.ToString());
            Console.WriteLine("Title: " + isTitleDifferent);

            return isDescriptionDifferent || isLanguageDifferent || isLengthDifferent || isReleaseDateDifferent || isTitleDifferent;
        }
    }
}
