using System;

namespace Application.Validation
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
            bool isReleaseDateDifferent = movie.ReleaseDate != adminMovieModel.ReleaseDate.ToString("dd-MM-yyyy");
            bool isTitleDifferent = movie.Title != adminMovieModel.Title;

            return isDescriptionDifferent || isLanguageDifferent || isLengthDifferent || isReleaseDateDifferent || isTitleDifferent;
        }
    }
}
