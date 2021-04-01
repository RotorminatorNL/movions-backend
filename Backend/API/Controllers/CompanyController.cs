using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CompanyController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("[action]")]
        public bool Create(string name, [FromBody]List<Domain.Movie> movies)
        {
            return new Company(_applicationDbContext).Create(name, movies);
        }

        [HttpGet("[action]/{id}")]
        public Domain.Company Read(int id)
        {
            return new Company(_applicationDbContext).Read(id);
        }

        [HttpGet("[action]")]
        public IEnumerable<Domain.Company> ReadAll()
        {
            return new Company(_applicationDbContext).ReadAll();
        }
    }
}
