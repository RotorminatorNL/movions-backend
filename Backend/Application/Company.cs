using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Company
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly CompanyValidation _companyValidation;

        public Company(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _companyValidation = new CompanyValidation();
        }

        public async Task<AdminCompanyModel> Create(AdminCompanyModel adminCompanyModel)
        {
            if (_companyValidation.IsInputValid(adminCompanyModel))
            {
                var company = new Domain.Company
                {
                    Name = adminCompanyModel.Name,
                    Type = adminCompanyModel.Type
                };

                _applicationDbContext.Companies.Add(company);

                await _applicationDbContext.SaveChangesAsync();

                return new AdminCompanyModel
                {
                    ID = company.ID,
                    Name = company.Name,
                    Type = company.Type
                };
            }

            return null;
        }

        public IEnumerable<CompanyModel> ReadAll()
        {
            return _applicationDbContext.Companies.ToList().Select(company => new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
            });
        }

        public CompanyModel Read(int id)
        {
            return _applicationDbContext.Companies.Select(company => new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
            }).FirstOrDefault(x => x.ID == id);
        }

        public async Task<AdminCompanyModel> Update(AdminCompanyModel adminCompanyModel) 
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(x => x.ID == adminCompanyModel.ID);

            company.Name = adminCompanyModel.Name;
            company.Type = adminCompanyModel.Type;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type
            };
        }

        public async Task<bool> Delete(int id) 
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Companies.Remove(company);
 
            try
            {
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
