using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer
{
    public class Movie
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Movie(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Domain.Movie> ReadAll()
        {
            return _applicationDbContext.Movies.ToList();
        }
    }
}
