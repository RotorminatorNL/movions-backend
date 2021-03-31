using BusinessContractLayer;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public bool Create(string name, [FromBody]List<Movie> movies)
        {
            return new CreateCompany(_applicationDbContext).Do(name, movies);
        }

        [HttpGet("[action]/{id}")]
        public Company Read(int id)
        {
            return new ReadCompany(_applicationDbContext).Do(id);
        }

        [HttpGet("[action]")]
        public IEnumerable<Company> ReadAll()
        {
            return new ReadAllCompanies(_applicationDbContext).Do();
        }
    }
}
