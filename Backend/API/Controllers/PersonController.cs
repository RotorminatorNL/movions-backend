using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PersonController : Controller
    {
        private readonly Person person;

        public PersonController(IApplicationDbContext applicationDbContext)
        {
            person = new Person(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] AdminPersonModel adminPersonModel)
        {
            var result = await person.Create(adminPersonModel);

            if (result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var result = await person.Read(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet()]
        public async Task<IActionResult> ReadAll()
        {
            var result = await person.ReadAll();

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminPersonModel adminPersonModel)
        {
            var result = await person.Update(adminPersonModel);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await person.Delete(id);

            if (result != true)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
