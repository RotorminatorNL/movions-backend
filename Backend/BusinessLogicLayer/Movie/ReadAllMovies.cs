using DataAccessLayer;
using BusinessContractLayer;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer
{
    public class ReadAllMovies
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReadAllMovies(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Movie> Do()
        {
            return _applicationDbContext.Movies.ToList();
        }
    }
}
