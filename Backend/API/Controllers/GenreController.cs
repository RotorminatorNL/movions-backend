using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GenreController : Controller
    {
        private readonly Genre genre;

        public GenreController(IApplicationDbContext applicationDbContext)
        {
            genre = new Genre(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] AdminGenreModel adminGenreModel)
        {
            var result = await genre.Create(adminGenreModel);

            if (result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var result = await genre.Read(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet()]
        public async Task<IActionResult> ReadAll()
        {
            var result = await genre.ReadAll();

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminGenreModel adminGenreModel)
        {
            var result = await genre.Update(adminGenreModel);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await genre.Delete(id);

            if (result != true)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
