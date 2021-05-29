using Application.AdminModels;
using System;

namespace Application.Validation
{
    public class PersonValidation
    {
        public bool IsInputValid(AdminPersonModel adminPersonModel)
        {
            bool isBirthDateOk = adminPersonModel.BirthDate != DateTime.Parse("1-1-0001 00:00:00");
            bool isBirthPlaceOk = !(adminPersonModel.BirthPlace == null || adminPersonModel.BirthPlace == "");
            bool isDescriptionOk = !(adminPersonModel.Description == null || adminPersonModel.Description == "");
            bool isFirstNameOk = !(adminPersonModel.FirstName == null || adminPersonModel.FirstName == "");
            bool isLastNameOk = !(adminPersonModel.LastName == null || adminPersonModel.LastName == "");

            return isBirthDateOk && isBirthPlaceOk && isDescriptionOk && isFirstNameOk && isLastNameOk;
        }
    }
}
