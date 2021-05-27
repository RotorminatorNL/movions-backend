using Application;
using Application.AdminModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly Company company;

        public CompanyController(IApplicationDbContext applicationDbContext)
        {
            company = new Company(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCompanyModel adminCompanyModel)
        {
            if (await company.Create(adminCompanyModel) is AdminCompanyModel result && result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            if (await company.Read(id) is CompanyModel result && result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            if (await company.ReadAll() is ICollection<CompanyModel> result && result.Count > 0)
            {
                return Ok(result);
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminCompanyModel adminCompanyModel)
        {
            if (await company.Update(adminCompanyModel) is AdminCompanyModel result && result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await company.Delete(id))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
