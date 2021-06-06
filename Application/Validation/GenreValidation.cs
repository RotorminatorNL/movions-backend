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
    }
}
