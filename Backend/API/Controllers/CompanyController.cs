using BusinessLogicLayer;
using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        private readonly Company Company;

        public CompanyController(ApplicationDbContext applicationDbContext)
        {
            Company = new Company(applicationDbContext);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(AdminCompanyModel adminCompanyModel)
        {
            return Ok(await Company.Create(adminCompanyModel));
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Company.Read(id));
        }

        [HttpGet("[action]")]
        public IActionResult ReadAll()
        {
            return Ok(Company.ReadAll());
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Company.Delete(id));
        }
    }
}
