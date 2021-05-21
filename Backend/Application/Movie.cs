using Application.Validation;
using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application
{
    public class Movie
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly MovieValidation _movieValidation;

        public Movie(IApplicationDbContext applicationDbContext)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");

            _applicationDbContext = applicationDbContext;
            _movieValidation = new MovieValidation();
        }

        public async Task<AdminMovieModel> Create(AdminMovieModel adminMovieModel)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");

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

                Console.WriteLine("------------------------------------------");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("--- Movie Console 'log' ---");

                Console.WriteLine("Movie releaseDate: " + movie.ReleaseDate);
                Console.WriteLine("ReturnData ReleaseDate: " + returnData.ReleaseDate);

                Console.WriteLine("------------------------------------------");
                Console.WriteLine("------------------------------------------");

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
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");

            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == adminMovieModel.ID);

            if (movie != null && _movieValidation.IsInputValid(adminMovieModel))
            {
                if (_movieValidation.IsInputDataDifferent(movie, adminMovieModel))
                {
                    movie.Description = adminMovieModel.Description;
                    movie.Length = adminMovieModel.Length;
                    movie.LanguageID = adminMovieModel.Language.ID;
                    movie.ReleaseDate = adminMovieModel.ReleaseDate.ToString("dd-MM-yyyy");
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
