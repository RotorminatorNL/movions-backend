using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CrewMemberController : Controller
    {
        private readonly CrewMember crewMember;

        public CrewMemberController(IApplicationDbContext applicationDbContext)
        {
            crewMember = new CrewMember(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] AdminCrewMemberModel adminCrewMemberModel)
        {
            var result = await crewMember.Create(adminCrewMemberModel);

            if (result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var result = await crewMember.Read(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet()]
        public async Task<IActionResult> ReadAll()
        {
            var result = await crewMember.ReadAll();

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminCrewMemberModel adminCrewMemberModel)
        {
            var result = await crewMember.Update(adminCrewMemberModel);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await crewMember.Delete(id);

            if (result != true)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
