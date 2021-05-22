using Domain.Enums;
using System;

namespace Application.Validation
{
    public class CompanyValidation
    {
        public bool IsInputValid(AdminCompanyModel adminCompanyModel)
        {
            bool isNameOk = !(adminCompanyModel.Name == null || adminCompanyModel.Name == "");
            bool isCompanyTypeOk = Enum.IsDefined(typeof(CompanyTypes), adminCompanyModel.Type);

            return isNameOk && isCompanyTypeOk;
        }

        public bool IsInputDifferent(Domain.Company company, AdminCompanyModel adminCompanyModel)
        {
            bool isNameOk = company.Name != adminCompanyModel.Name;
            bool isCompanyTypeOk = company.Type != adminCompanyModel.Type;

            return isNameOk || isCompanyTypeOk;
        }
    }
}
