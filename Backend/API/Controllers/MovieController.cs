using BusinessContractLayer;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MovieController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet("[action]")]
        public IEnumerable<Movie> ReadAll()
        {
            return new ReadAllMovies(_applicationDbContext).Do();
        }
    }
}
