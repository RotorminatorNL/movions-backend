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

        private NotFoundObjectResult GetCustomNotFound(int id)
        {
            switch (id)
            {
                case -1:
                    {
                        ModelState.AddModelError("MovieID", "Does not exist.");
                        break;
                    }
                case -2:
                    {
                        ModelState.AddModelError("CompanyID", "Does not exist.");
                        break;
                    }
                case -3:
                    {
                        ModelState.AddModelError("CompanyID", "Does not exist.");
                        ModelState.AddModelError("MovieID", "Does not exist.");
                        break;
                    }
                default:
                    ModelState.AddModelError("CompanyMovieID", "Does not exist.");
                    break;
            }

            return NotFound(new ValidationProblemDetails(ModelState));
        }

        public CompanyController(IApplicationDbContext applicationDbContext)
        {
            company = new Company(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCompanyModel adminCompanyModel)
        {
            if (await company.Create(adminCompanyModel) is CompanyModel result && result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost("{id}/movies")]
        public async Task<IActionResult> ConnectMovie(int id, [FromBody] int movieId)
        {
            var result = await company.ConnectMovie(new AdminCompanyMovieModel { CompanyID = id, MovieID = movieId });

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return Ok(result);
                }

                return GetCustomNotFound(result.ID);
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
            if (await company.Update(adminCompanyModel) is CompanyModel result && result != null)
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

        [HttpDelete("{id}/movies/{movieID}")]
        public async Task<IActionResult> DisconnectMovie(int id, int MovieID)
        {
            var result = await company.DisconnectMovie(new AdminCompanyMovieModel { CompanyID = id, MovieID = MovieID });

            if (result != null)
            {
                if (result.ID == 0)
                {
                    return Ok(result);
                }

                return GetCustomNotFound(result.ID);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
