using Application.AdminModels;

namespace Application.Validation
{
    public class GenreValidation
    {
        public bool IsInputValid(AdminGenreModel adminGenreModel)
        {
            bool isNameOk = !(adminGenreModel.Name == null || adminGenreModel.Name == "");

            return isNameOk;
        }

        public bool IsInputDifferent(Domain.Genre genre, AdminGenreModel adminGenreModel)
        {
            bool isNameOk = genre.Name != adminGenreModel.Name;

            return isNameOk;
        }
    }
}
