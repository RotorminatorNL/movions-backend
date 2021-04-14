using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Language
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Language(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminLanguageModel> Create(AdminLanguageModel adminLanguageModel)
        {
            var language = new Domain.Language
            {
                Name = adminLanguageModel.Name
            };

            _applicationDbContext.Languages.Add(language);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminLanguageModel
            {
                ID = language.ID,
                Name = language.Name
            };
        }

        public IEnumerable<LanguageModel> ReadAll()
        {
            return _applicationDbContext.Languages
            .Include(languages => languages.Movies)
            .ToList()
            .Select(language => new LanguageModel
            {
                ID = language.ID,
                Name = language.Name,
                Movies = language.Movies.Select(movie => new MovieModel
                {
                    ID = movie.ID,
                    Description = movie.Description,
                    Length = movie.Length,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    //Companies = movie.Companies.Select(company => new CompanyModel
                    //{
                    //    ID = company.ID,
                    //    Name = company.Name,
                    //    Type = company.Type.ToString()
                    //}),
                    //Crew = movie.Crew.Select(crewRole => new CrewRoleModel
                    //{
                    //    ID = crewRole.ID,
                    //    CharacterName = crewRole.CharacterName,
                    //    Role = crewRole.Role.ToString()
                    //}),
                    //Genres = movie.Genres.Select(genre => new GenreModel
                    //{
                    //    ID = genre.ID,
                    //    Name = genre.Name
                    //}),
                }),
            });
        }

        public LanguageModel Read(int id)
        {
            return _applicationDbContext.Genres.Select(genre => new LanguageModel
            {
                ID = genre.ID,
                Name = genre.Name,
                Movies = genre.Movies.Select(movie => new MovieModel
                {
                    ID = movie.ID,
                    Description = movie.Description,
                    Length = movie.Length,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    Companies = movie.Companies.Select(company => new CompanyModel
                    {
                        ID = company.ID,
                        Name = company.Name,
                        Type = company.Type.ToString()
                    }),
                    Crew = movie.Crew.Select(crewRole => new CrewRoleModel
                    {
                        ID = crewRole.ID,
                        CharacterName = crewRole.CharacterName,
                        Role = crewRole.Role.ToString()
                    }),
                    Genres = movie.Genres.Select(genre => new GenreModel
                    {
                        ID = genre.ID,
                        Name = genre.Name
                    }),
                }),
            }).FirstOrDefault(c => c.ID == id);
        }

        public async Task<AdminLanguageModel> Update(AdminLanguageModel adminLanguageModel) 
        {
            var language = _applicationDbContext.Languages.FirstOrDefault(x => x.ID == adminLanguageModel.ID);

            language.Name = adminLanguageModel.Name;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminLanguageModel
            {
                ID = language.ID,
                Name = language.Name
            };
        }

        public async Task<bool> Delete(int id) 
        {
            var language = _applicationDbContext.Languages.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Languages.Remove(language);
 
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
