using Application.Validation;
using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Genre
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly GenreValidation _genreValidation;

        public Genre(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _genreValidation = new GenreValidation();
        }

        public async Task<AdminGenreModel> Create(AdminGenreModel adminGenreModel)
        {
            if (_genreValidation.IsInputValid(adminGenreModel))
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

            return null;
        }

        public GenreModel Read(int id)
        {
            return _applicationDbContext.Genres.Select(genre => new GenreModel
            {
                ID = genre.ID,
                Name = genre.Name,
            }).FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<GenreModel> ReadAll()
        {
            return _applicationDbContext.Genres.Select(genre => new GenreModel
            {
                ID = genre.ID,
                Name = genre.Name,
            }).ToList();
        }

        public async Task<AdminGenreModel> Update(AdminGenreModel adminGenreModel)
        {
            var genre = _applicationDbContext.Genres.FirstOrDefault(x => x.ID == adminGenreModel.ID);
            
            if (genre != null && _genreValidation.IsInputValid(adminGenreModel))
            {
                if (_genreValidation.IsInputDifferent(genre, adminGenreModel))
                {
                    genre.Name = adminGenreModel.Name;

                    await _applicationDbContext.SaveChangesAsync();

                    return new AdminGenreModel
                    {
                        ID = genre.ID,
                        Name = genre.Name
                    };
                }

                return new AdminGenreModel();
            }

            return null;
        }

        public async Task<bool> Delete(int id) 
        {
            var genre = _applicationDbContext.Genres.FirstOrDefault(x => x.ID == id);

            if (genre != null)
            {
                _applicationDbContext.Genres.Remove(genre);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }
            
            return false;
            
        }
    }
}
