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

        public async Task<Domain.Company> Create(AdminCompanyModel adminCompanyModel)
        {
            var c = new Domain.Company
            {
                Name = adminCompanyModel.Name,
                Type = (Domain.Company.Types)adminCompanyModel.Type
            };

            _applicationDbContext.Companies.Add(c);

            await _applicationDbContext.SaveChangesAsync();

            return c;
        }

        public IEnumerable<CompanyModel> ReadAll()
        {
            return _applicationDbContext.Companies.Include(cs => cs.Movies).ToList().Select(c => new CompanyModel
            {
                ID = c.ID,
                Name = c.Name,
                Type = c.Type.ToString(),
                Movies = c.Movies.Select(m => new MovieModel
                {
                    ID = m.ID,
                    Description = m.Description,
                    Length = m.Length,
                    ReleaseDate = m.ReleaseDate,
                    Title = m.Title,
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
            return _applicationDbContext.Companies.Select(c => new CompanyModel
            {
                ID = c.ID,
                Name = c.Name,
                Type = c.Type.ToString(),
                Movies = c.Movies.Select(m => new MovieModel
                {
                    ID = m.ID,
                    Description = m.Description,
                    Length = m.Length,
                    ReleaseDate = m.ReleaseDate,
                    Title = m.Title,
                    Crew = m.Crew.Select(cr => new CrewRoleModel { 
                        ID = cr.ID,
                        CharacterName = cr.CharacterName,
                        Role = cr.Role.ToString()
                    }),
                    Genres = m.Genres.Select(g => new GenreModel
                    {
                        ID = g.ID,
                        Name = g.Name
                    }),
                    Language = new LanguageModel
                    {
                        ID = m.Language.ID,
                        Name = m.Language.Name
                    }
                }),
            }).FirstOrDefault(c => c.ID == id);
        }

        public void Update() { }

        public async Task<bool> Delete(int id) 
        {
            var c = _applicationDbContext.Companies.FirstOrDefault(c => c.ID == id);

            _applicationDbContext.Companies.Remove(c);
 
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
