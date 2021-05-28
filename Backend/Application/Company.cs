using Application.AdminModels;
using Application.Validation;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using PersistenceInterface;
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

        public async Task<AdminCompanyModel> AddMovie(AdminCompanyModel adminCompanyModel)
        {
            return adminCompanyModel;
        }

        public async Task<CompanyModel> Read(int id)
        {
            return await _applicationDbContext.Companies.Select(company => new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
            }).FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<CompanyModel>> ReadAll()
        {
            return await _applicationDbContext.Companies.Select(company => new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
            }).ToListAsync();
        }

        public async Task<AdminCompanyModel> Update(AdminCompanyModel adminCompanyModel)
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(x => x.ID == adminCompanyModel.ID);

            if (company != null && _companyValidation.IsInputValid(adminCompanyModel))
            {
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

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(x => x.ID == id);

            if (company != null)
            {
                _applicationDbContext.Companies.Remove(company);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }
                
            return false;
        }
    }
}
