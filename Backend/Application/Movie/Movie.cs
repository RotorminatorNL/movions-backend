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

        public Movie(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminMovieModel> Create(AdminMovieModel adminMovieModel)
        {
            if(new Validation().MovieCheck(adminMovieModel))
            {
                return null;
            }

            var movie = new Domain.Movie
            {
                Description = adminMovieModel.Description,
                Length = adminMovieModel.Length,
                ReleaseDate = adminMovieModel.ReleaseDate.ToShortDateString(),
                Title = adminMovieModel.Title
            };

            _applicationDbContext.Movies.Add(movie);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminMovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            };
        }

        public IEnumerable<MovieModel> ReadAll()
        {
            return _applicationDbContext.Movies.ToList().Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title,
            });
        }

        public MovieModel Read(int id)
        {
            return _applicationDbContext.Movies.ToList().Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title,
            }).FirstOrDefault(x => x.ID == id);
        }

        public async Task<AdminMovieModel> Update(AdminMovieModel adminMovieModel)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == adminMovieModel.ID);

            movie.Description = adminMovieModel.Description;
            movie.Length = adminMovieModel.Length;
            movie.ReleaseDate = adminMovieModel.ReleaseDate.ToShortDateString();
            movie.Title = adminMovieModel.Title;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminMovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
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
