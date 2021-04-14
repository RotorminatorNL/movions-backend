using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Genre
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Genre(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminGenreModel> Create(AdminGenreModel adminGenreModel)
        {
            var genre = new Domain.Genre
            {
                Name = adminGenreModel.Name
            };

            _applicationDbContext.Genres.Add(genre);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminGenreModel
            {
                ID = genre.ID,
                Name = genre.Name
            };
        }

        public IEnumerable<GenreModel> ReadAll()
        {
            return _applicationDbContext.Genres.Include(genres => genres.Movies).ToList().Select(genre => new GenreModel
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
                    //Language = new LanguageModel
                    //{
                    //    ID = movie.Language.ID,
                    //    Name = movie.Language.Name
                    //}
                }),
            });
        }

        public GenreModel Read(int id)
        {
            return _applicationDbContext.Genres.Select(genre => new GenreModel
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
                    Crew = movie.Crew.Select(crewRole => new CrewRoleModel { 
                        ID = crewRole.ID,
                        CharacterName = crewRole.CharacterName,
                        Role = crewRole.Role.ToString()
                    }),
                    Language = new LanguageModel
                    {
                        ID = movie.Language.ID,
                        Name = movie.Language.Name
                    }
                }),
            }).FirstOrDefault(c => c.ID == id);
        }

        public async Task<AdminGenreModel> Update(AdminGenreModel adminGenreModel) 
        {
            var genre = _applicationDbContext.Genres.FirstOrDefault(x => x.ID == adminGenreModel.ID);

            genre.Name = adminGenreModel.Name;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminGenreModel
            {
                ID = genre.ID,
                Name = genre.Name
            };
        }

        public async Task<bool> Delete(int id) 
        {
            var genre = _applicationDbContext.Genres.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Genres.Remove(genre);
 
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
