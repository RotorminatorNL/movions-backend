using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validation
{
    public class LanguageValidation
    {
        public bool IsInputValid(AdminLanguageModel adminLanguageModel)
        {
            bool isNameOk = !(adminLanguageModel.Name == null || adminLanguageModel.Name == "");

            return isNameOk;
        }

        public bool IsInputDifferent(Domain.Language language, AdminLanguageModel adminLanguageModel)
        {
            bool isNameOk = language.Name != adminLanguageModel.Name;

            return isNameOk;
        }

        public bool CheckValidations(Domain.Language language, AdminLanguageModel adminLanguageModel)
        {
            return IsInputValid(adminLanguageModel) && IsInputDifferent(language, adminLanguageModel);
        }
    }
}
