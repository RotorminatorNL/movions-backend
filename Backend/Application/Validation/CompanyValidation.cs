using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class CompanyValidation
    {
        public bool IsInputValid(AdminCompanyModel adminCompanyModel)
        {
            bool isNameOk = !(adminCompanyModel.Name == null || adminCompanyModel.Name == "");
            bool isCompanyTypeOk = Enum.IsDefined(typeof(CompanyTypes), adminCompanyModel.Type);

            return isNameOk && isCompanyTypeOk;
        }
    }
}
