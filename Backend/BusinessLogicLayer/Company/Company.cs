using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Company
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Company(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminCompanyModel> Create(AdminCompanyModel adminCompanyModel)
        {
            var company = new Domain.Company
            {
                Name = adminCompanyModel.Name,
                Type = (Domain.Company.Types)adminCompanyModel.Type
            };

            _applicationDbContext.Companies.Add(company);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCompanyModel 
            {
                ID = company.ID,
                Name = company.Name,
                Type = (AdminCompanyModel.Types)company.Type
            };
        }

        public IEnumerable<CompanyModel> ReadAll()
        {
            return _applicationDbContext.Companies.Include(companies => companies.Movies).ToList().Select(company => new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
                Movies = company.Movies.Select(movie => new MovieModel
                {
                    ID = movie.ID,
                    Description = movie.Description,
                    Length = movie.Length,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    //Crew = m.Crew.Select(cr => new CrewRoleModel
                    //{
                    //    ID = cr.ID,
                    //    CharacterName = cr.CharacterName,
                    //    Role = cr.Role.ToString()
                    //}),
                    //Genres = m.Genres.Select(g => new GenreModel
                    //{
                    //    ID = g.ID,
                    //    Name = g.Name
                    //}),
                    //Language = new LanguageModel
                    //{
                    //    ID = m.Language.ID,
                    //    Name = m.Language.Name
                    //}
                }),
            });
        }

        public CompanyModel Read(int id)
        {
            return _applicationDbContext.Companies.Select(company => new CompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = company.Type.ToString(),
                Movies = company.Movies.Select(movie => new MovieModel
                {
                    ID = movie.ID,
                    Description = movie.Description,
                    Length = movie.Length,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    Crew = movie.Crew.Select(crewRole => new CrewRoleModel { 
                        ID = crewRole.ID,
                        CharacterName = crewRole.CharacterName,
                        Role = crewRole.Role.ToString()
                    }),
                    Genres = movie.Genres.Select(genre => new GenreModel
                    {
                        ID = genre.ID,
                        Name = genre.Name
                    }),
                    Language = new LanguageModel
                    {
                        ID = movie.Language.ID,
                        Name = movie.Language.Name
                    }
                }),
            }).FirstOrDefault(c => c.ID == id);
        }

        public async Task<AdminCompanyModel> Update(AdminCompanyModel adminCompanyModel) 
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(x => x.ID == adminCompanyModel.ID);

            company.Name = adminCompanyModel.Name;
            company.Type = (Domain.Company.Types)adminCompanyModel.Type;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCompanyModel
            {
                ID = company.ID,
                Name = company.Name,
                Type = (AdminCompanyModel.Types)company.Type
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
