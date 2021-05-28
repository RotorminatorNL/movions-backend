using Application.AdminModels;

namespace Application.Validation
{
    public class LanguageValidation
    {
        public bool IsInputValid(AdminLanguageModel adminLanguageModel)
        {
            bool isNameOk = !(adminLanguageModel.Name == null || adminLanguageModel.Name == "");

            return isNameOk;
        }
    }
}
