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

        public Genre(IApplicationDbContext applicationDbContext)
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
            return _applicationDbContext.Genres.ToList().Select(genre => new GenreModel
            {
                ID = genre.ID,
                Name = genre.Name,
            });
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
