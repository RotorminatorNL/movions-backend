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

        private MovieModel GetMovieModelWithErrorID(object genre, object movie)
        {
            if (genre == null && movie != null)
            {
                return new MovieModel
                {
                    ID = -1
                };
            }

            if (genre != null && movie == null)
            {
                return new MovieModel
                {
                    ID = -2
                };
            }

            if (genre == null && movie == null)
            {
                return new MovieModel
                {
                    ID = -3
                };
            }

            return new MovieModel
            {
                ID = -4
            };
        }

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
                    LanguageID = adminMovieModel.LanguageID,
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

        public async Task<MovieModel> ConnectGenre(AdminGenreMovieModel adminGenreMovieModel)
        {
            var genre = await _applicationDbContext.Genres.FirstOrDefaultAsync(c => c.ID == adminGenreMovieModel.GenreID);
            var movie = await _applicationDbContext.Movies.FirstOrDefaultAsync(c => c.ID == adminGenreMovieModel.MovieID);

            if (genre != null && movie != null)
            {
                var doesConnectionExist = await _applicationDbContext.GenreMovies.FirstOrDefaultAsync(g => g.GenreID == genre.ID && g.MovieID == movie.ID);

                if (doesConnectionExist == null)
                {
                    var genreMovie = new Domain.GenreMovie
                    {
                        GenreID = adminGenreMovieModel.GenreID,
                        MovieID = adminGenreMovieModel.MovieID
                    };

                    _applicationDbContext.GenreMovies.Add(genreMovie);
                    await _applicationDbContext.SaveChangesAsync();

                    return await Read(adminGenreMovieModel.MovieID);
                }

                return GetMovieModelWithErrorID(genre, movie);
            }

            return GetMovieModelWithErrorID(genre, movie);
        }

        public async Task<MovieModel> Read(int id)
        {
            return await _applicationDbContext.Movies.Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Genres = movie.Genres.Select(genre => new GenreModel 
                { 
                    ID = genre.Genre.ID,
                    Name = genre.Genre.Name
                }),
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
                movie.LanguageID = adminMovieModel.LanguageID;
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

        public async Task<MovieModel> DisconnectGenre(AdminGenreMovieModel adminGenreMovieModel)
        {
            var genre = await _applicationDbContext.Genres.FirstOrDefaultAsync(c => c.ID == adminGenreMovieModel.GenreID);
            var movie = await _applicationDbContext.Movies.FirstOrDefaultAsync(c => c.ID == adminGenreMovieModel.MovieID);

            if (genre != null && movie != null)
            {
                var genreMovie = _applicationDbContext
                                    .GenreMovies
                                    .FirstOrDefault(c => c.GenreID == genre.ID && c.MovieID == movie.ID);

                if (genreMovie != null)
                {
                    _applicationDbContext.GenreMovies.Remove(genreMovie);
                    await _applicationDbContext.SaveChangesAsync();

                    return new MovieModel();
                }
            }

            return GetMovieModelWithErrorID(genre, movie);
        }
    }
}
