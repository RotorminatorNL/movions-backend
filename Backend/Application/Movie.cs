using Application.AdminModels;
using Application.Validation;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Movie
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly MovieValidation _movieValidation;

        public Movie(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _movieValidation = new MovieValidation();
        }

        public async Task<MovieModel> Create(AdminMovieModel adminMovieModel)
        {
            if (_movieValidation.IsInputValid(adminMovieModel))
            {
                var movie = new Domain.Movie
                {
                    Description = adminMovieModel.Description,
                    LanguageID = adminMovieModel.Language.ID,
                    Length = adminMovieModel.Length,
                    ReleaseDate = adminMovieModel.ReleaseDate.ToString("dd-MM-yyyy"),
                    Title = adminMovieModel.Title
                };

                _applicationDbContext.Movies.Add(movie);
                await _applicationDbContext.SaveChangesAsync();

                return await Read(movie.ID);
            }

            return null;
        }

        public async Task<MovieModel> Read(int id)
        {
            return await _applicationDbContext.Movies.Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Language = new LanguageModel
                {
                    ID = movie.Language.ID,
                    Name = movie.Language.Name
                },
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            }).FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<MovieModel>> ReadAll()
        {
            return await _applicationDbContext.Movies.Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            }).ToListAsync();
        }

        public async Task<MovieModel> Update(AdminMovieModel adminMovieModel)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == adminMovieModel.ID);

            if (movie != null && _movieValidation.IsInputValid(adminMovieModel))
            {
                movie.Description = adminMovieModel.Description;
                movie.Length = adminMovieModel.Length;
                movie.LanguageID = adminMovieModel.Language.ID;
                movie.ReleaseDate = adminMovieModel.ReleaseDate.ToString("dd-MM-yyyy");
                movie.Title = adminMovieModel.Title;

                await _applicationDbContext.SaveChangesAsync();

                return await Read(movie.ID);
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == id);

            if (movie != null)
            {
                _applicationDbContext.Movies.Remove(movie);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
