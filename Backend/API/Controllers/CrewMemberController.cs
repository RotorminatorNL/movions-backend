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
    public class CrewMemberController : Controller
    {
        private readonly CrewMember crewMember;

        private BadRequestObjectResult GetCustomBadRequest(int id)
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
                        ModelState.AddModelError("PersonID", "Does not exist.");
                        break;
                    }
                default:
                    ModelState.AddModelError("MovieID", "Does not exist.");
                    ModelState.AddModelError("PersonID", "Does not exist.");
                    break;
            }

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        public CrewMemberController(IApplicationDbContext applicationDbContext)
        {
            crewMember = new CrewMember(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCrewMemberModel adminCrewMemberModel)
        {
            var result = await crewMember.Create(adminCrewMemberModel);

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
                }

                return GetCustomBadRequest(result.ID);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            if (await crewMember.Read(id) is CrewMemberModel result && result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            if (await crewMember.ReadAll() is ICollection<CrewMemberModel> result && result.Count > 0)
            {
                return Ok(result);
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminCrewMemberModel adminCrewMemberModel)
        {
            var result = await crewMember.Update(adminCrewMemberModel);

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return Ok(result);
                }

                return GetCustomBadRequest(result.ID);
            }


            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await crewMember.Delete(id))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
