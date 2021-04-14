using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Movie
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Movie(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminMovieModel> Create(AdminMovieModel adminMovieModel)
        {
            var movie = new Domain.Movie
            {
                Description = adminMovieModel.Description,
                Length = adminMovieModel.Length,
                ReleaseDate = adminMovieModel.ReleaseDate,
                Title = adminMovieModel.Title
            };

            _applicationDbContext.Movies.Add(movie);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminMovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = movie.ReleaseDate,
                Title = movie.Title
            };
        }

        public IEnumerable<MovieModel> ReadAll()
        {
            return _applicationDbContext.Movies
            .Include(movies => movies.Companies)
            .Include(movies => movies.Crew)
            .Include(movies => movies.Genres)
            .Include(movies => movies.Language).ToList().Select(movie => new MovieModel
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
                Language = new LanguageModel
                {
                    ID = movie.Language.ID,
                    Name = movie.Language.Name
                }
            });
        }

        public MovieModel Read(int id)
        {
            return _applicationDbContext.Movies
            .Include(movies => movies.Companies)
            .Include(movies => movies.Crew)
            .Include(movies => movies.Genres)
            .Include(movies => movies.Language).ToList().Select(movie => new MovieModel
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
                Language = new LanguageModel
                {
                    ID = movie.Language.ID,
                    Name = movie.Language.Name
                }
            }).FirstOrDefault(c => c.ID == id);
        }

        public async Task<AdminMovieModel> Update(AdminMovieModel adminMovieModel)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == adminMovieModel.ID);

            movie.Description = adminMovieModel.Description;
            movie.Length = adminMovieModel.Length;
            movie.ReleaseDate = adminMovieModel.ReleaseDate;
            movie.Title = adminMovieModel.Title;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminMovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = movie.ReleaseDate,
                Title = movie.Title
            };
        }

        public async Task<bool> Delete(int id)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Movies.Remove(movie);

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
