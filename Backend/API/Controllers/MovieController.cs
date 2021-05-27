using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Collections.Generic;
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
            if (await movie.Create(adminMovieModel) is AdminMovieModel result && result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            if (await movie.Read(id) is MovieModel result && result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            if (await movie.ReadAll() is ICollection<MovieModel> result && result.Count > 0)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminMovieModel adminMovieModel)
        {
            if (await movie.Update(adminMovieModel) is AdminMovieModel result && result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await movie.Delete(id))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
