using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LanguageController : Controller
    {
        private readonly Language language;

        public LanguageController(IApplicationDbContext applicationDbContext)
        {
            language = new Language(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] AdminLanguageModel adminLanguageModel)
        {
            var result = await language.Create(adminLanguageModel);

            if (result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var result = await language.Read(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet()]
        public async Task<IActionResult> ReadAll()
        {
            var result = await language.ReadAll();

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminLanguageModel adminLanguageModel)
        {
            var result = await language.Update(adminLanguageModel);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await language.Delete(id);

            if (result != true)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
