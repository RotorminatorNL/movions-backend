using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {
        private readonly Movie movie;

        public MovieController(IApplicationDbContext applicationDbContext)
        {
            movie = new Movie(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminMovieModel adminMovieModel)
        {
            var result = await movie.Create(adminMovieModel);

            if (result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var result = await movie.Read(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            var result = await movie.ReadAll();

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminMovieModel adminMovieModel)
        {
            var result = await movie.Update(adminMovieModel);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await movie.Delete(id);

            if (result != true)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
