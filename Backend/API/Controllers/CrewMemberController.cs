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

        public CrewMemberController(IApplicationDbContext applicationDbContext)
        {
            crewMember = new CrewMember(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCrewMemberModel adminCrewMemberModel)
        {
            if (await crewMember.Create(adminCrewMemberModel) is AdminCrewMemberModel result && result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
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

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminCrewMemberModel adminCrewMemberModel)
        {
            if (await crewMember.Update(adminCrewMemberModel) is AdminCrewMemberModel result && result != null)
            {
                return Ok(result);
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
