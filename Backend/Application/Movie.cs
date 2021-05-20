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

        public async Task<AdminMovieModel> Create(AdminMovieModel adminMovieModel)
        {
            if(_movieValidation.IsInputValid(adminMovieModel))
            {
                var movie = new Domain.Movie
                {
                    Description = adminMovieModel.Description,
                    LanguageID = adminMovieModel.Language.ID,
                    Length = adminMovieModel.Length,
                    ReleaseDate = adminMovieModel.ReleaseDate.ToShortDateString(),
                    Title = adminMovieModel.Title
                };

                _applicationDbContext.Movies.Add(movie);
                await _applicationDbContext.SaveChangesAsync();

                var returnData = new AdminMovieModel
                {
                    ID = movie.ID,
                    Description = movie.Description,
                    Language = new AdminLanguageModel
                    {
                        ID = movie.Language.ID,
                        Name = movie.Language.Name
                    },
                    Length = movie.Length,
                    ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                    Title = movie.Title
                };

                return returnData;
            }

            return null;
        }

        public MovieModel Read(int id)
        {
            return _applicationDbContext.Movies.Select(movie => new MovieModel
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
            }).FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<MovieModel> ReadAll()
        {
            return _applicationDbContext.Movies.Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            }).ToList();
        }

        public async Task<AdminMovieModel> Update(AdminMovieModel adminMovieModel)
        {
            if (adminMovieModel.ID != 0 && _movieValidation.IsInputValid(adminMovieModel))
            {
                var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == adminMovieModel.ID);

                if (_movieValidation.IsInputDataDifferent(movie, adminMovieModel))
                {
                    movie.Description = adminMovieModel.Description;
                    movie.Length = adminMovieModel.Length;
                    movie.LanguageID = adminMovieModel.Language.ID;
                    movie.ReleaseDate = adminMovieModel.ReleaseDate.ToString("yyyy-MM-dd");
                    movie.Title = adminMovieModel.Title;

                    await _applicationDbContext.SaveChangesAsync();

                    return new AdminMovieModel
                    {
                        ID = movie.ID,
                        Description = movie.Description,
                        Language = new AdminLanguageModel
                        {
                            ID = movie.Language.ID,
                            Name = movie.Language.Name
                        },
                        Length = movie.Length,
                        ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                        Title = movie.Title
                    };
                }

                return new AdminMovieModel();
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == id);
                _applicationDbContext.Movies.Remove(movie);
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
